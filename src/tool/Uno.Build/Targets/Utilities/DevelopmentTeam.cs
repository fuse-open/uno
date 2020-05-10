using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace Uno.Build.Targets.Utilities
{
    class DevTeam
    {
        public readonly string Name;
        public readonly string OrganizationName;
        public readonly string OrganizationalUnit; // Cert: OU

        public DevTeam(string name, string organizationName, string organizationalUnit)
        {
            Name = name;
            OrganizationName = organizationName;
            OrganizationalUnit = organizationalUnit;
        }

        public override string ToString()
        {
            return "{ Name: " + Name + ", OrganizationName: " + OrganizationName + ", OrganizationalUnit: " + OrganizationalUnit + " }";
        }
    }

    class DevelopmentTeamExtractorFailure : Exception
    {
        public readonly int ErrorCode;
        public DevelopmentTeamExtractorFailure(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }

    class DevelopmentTeamExtractor
    {
        private static readonly object _nsApplicationInitLock = new object();
        private static bool _nsApplicationInitialized;

        readonly byte[] _codeSigningUsageBytes = new byte[]{ 0x2b, 0x6, 0x1, 0x5, 0x5, 0x7, 0x3, 0x3 };
        const string _securityLibraryPath = "/System/Library/Frameworks/Security.framework/Versions/Current/Security";
        readonly IntPtr _securityLibrary;

        public DevelopmentTeamExtractor()
        {
            lock (_nsApplicationInitLock)
            {
                if (!_nsApplicationInitialized)
                {
                    NSApplication.Init();
                    _nsApplicationInitialized = true;
                }
            }

            _securityLibrary = Dlfcn.dlopen(_securityLibraryPath, 0);
        }

        public IEnumerable<DevTeam> FindAllDevelopmentTeams()
        {
            var query = NSDictionary.FromObjectsAndKeys(new object[]
                {
                    SecClassCertificate,
                    SecMatchLimitAll,
                    new NSNumber(true),
                    new NSNumber(true),
                    new NSNumber(true)
                },
                new object[]
                {
                    SecClassKey,
                    SecMatchLimit,
                    SecReturnRef,
                    SecMatchTrustedOnly,
                    SecAttrCanSign
                });


            var certificationsRefPtr = IntPtr.Zero;
            var error = SecItemCopyMatching(query.Handle, ref certificationsRefPtr);
            if (error != 0)
            {
                throw new DevelopmentTeamExtractorFailure("Failed to search for developer identities", error);
            }

            var devTeams = new List<DevTeam>();
            var certificationsRef = Runtime.GetNSObject<NSArray>(certificationsRefPtr);

            for (uint i = 0; i < certificationsRef.Count; ++i)
            {
                var devTeam = PareDevelopmentTeam(certificationsRef.ValueAt (i));
                if(devTeam != null)
                    devTeams.Add(devTeam);
            }

            return devTeams;
        }

        DevTeam PareDevelopmentTeam(IntPtr certificationRef)
        {
            var certificateValuesRef = SecCertificateCopyValues(certificationRef, IntPtr.Zero, IntPtr.Zero);
            if (certificateValuesRef == IntPtr.Zero)
            {
                throw new DevelopmentTeamExtractorFailure("Failed to copy values from certificate", -1);
            }

            var certificateValues = Runtime.GetNSObject<NSDictionary>(certificateValuesRef);

            // Check if Extended Key Usage is Codesigning
            var extendedKeyUsage = (NSDictionary)certificateValues.ValueForKey (SecOIDExtendedKeyUsage);
            if (extendedKeyUsage == null)
                return null;

            var extendedKeyUsageValues = (NSArray)extendedKeyUsage.ValueForKey (SecPropertyKeyValue);
            for (uint i = 0; i < extendedKeyUsageValues.Count; ++i)
            {
                var keyUsageData = Runtime.GetNSObject<NSData>(extendedKeyUsageValues.ValueAt(i));
                if (!CompareData (keyUsageData, _codeSigningUsageBytes))
                    return null;
            }

            var x509SubName = (NSDictionary)certificateValues.ValueForKey(SecOIDX509V1SubjectName);
            var x509Values = (NSArray)x509SubName.ValueForKey(SecPropertyKeyValue);

            NSString ouId = TryFind(x509Values, SecOIDOrganizationalUnitName);
            NSString organizationName = TryFind(x509Values, SecOIDOrganizationName);
            NSString commonName = TryFind(x509Values, SecOIDCommonName);

            return new DevTeam(commonName, organizationName, ouId);
        }

        bool CompareData(NSData a, byte[] bytes)
        {
            if (a.Length != (nuint)bytes.Length)
                return false;

            for (var j = 0; j < (uint)a.Length; ++j)
            {
                if (a[j] != _codeSigningUsageBytes[j])
                    return false;
            }

            return true;
        }

        NSString TryFind(NSArray x509Values, NSString key)
        {
            for (uint i = 0; i < x509Values.Count; ++i)
            {
                var curCertValue = Runtime.GetNSObject<NSDictionary>(x509Values.ValueAt(i));
                var valueLabel = (NSString)curCertValue.ValueForKey(SecPropertyKeyLabel);
                if (valueLabel.Compare(key) == NSComparisonResult.Same)
                    return (NSString)curCertValue.ValueForKey(SecPropertyKeyValue);
            }

            return null;
        }

        NSString SecClassKey { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecClass")); } }
        NSString SecReturnRef { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecReturnRef")); } }
        NSString SecMatchLimit { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecMatchLimit")); } }
        NSString SecReturnAttributes { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecReturnAttributes")); } }
        NSString SecClassCertificate { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecClassCertificate")); } }
        NSString SecMatchLimitAll { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecMatchLimitAll")); } }
        NSString SecOIDX509V1SubjectName { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecOIDX509V1SubjectName")); } }
        NSString SecPropertyKeyValue { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecPropertyKeyValue")); } }
        NSString SecPropertyKeyLabel { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecPropertyKeyLabel")); } }
        NSString SecOIDOrganizationalUnitName { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecOIDOrganizationalUnitName")); } }
        NSString SecOIDOrganizationName { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecOIDOrganizationName")); } }
        NSString SecOIDCommonName { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecOIDCommonName")); } }
        NSString SecMatchTrustedOnly { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecMatchTrustedOnly")); } }
        NSString SecAttrCanSign { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecAttrCanSign")); } }
        NSString SecOIDExtendedKeyUsage { get { return Runtime.GetNSObject<NSString>(Dlfcn.GetIntPtr(_securityLibrary, "kSecOIDExtendedKeyUsage")); } }

        [DllImport(_securityLibraryPath)]
        public static extern int SecItemCopyMatching(IntPtr queryDict, ref IntPtr result);

        [DllImport(_securityLibraryPath)]
        public static extern int SecIdentityCopyCertificate(IntPtr identityRef, ref IntPtr certRef);

        [DllImport(_securityLibraryPath)]
        public static extern IntPtr SecCertificateCopyValues(IntPtr certRef, IntPtr keys, IntPtr errors);
    }
}