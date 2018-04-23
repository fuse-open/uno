# Use by adding e.g. "command script import ~/uno/scripts/unolldb.py" to ~/.lldbinit
import lldb
from sets import Set

moduleName = __name__
logFile = '/tmp/' + moduleName + '.log'

unoPrimitiveTypes = {
        "Uno.Long": "int64_t",
        "Uno.Int": "int32_t",
        "Uno.Short": "int16_t",
        "Uno.SByte": "int8_t",
        "Uno.ULong": "uint64_t",
        "Uno.UInt": "uint32_t",
        "Uno.UShort": "uint16_t",
        "Uno.Byte": "uint8_t",
        "Uno.Bool": "bool",
        "Uno.Double": "double",
        "Uno.Float": "float"
}

def reverseString(s):
    return s[::-1]

def rreplace(str, x, y, max):
    return reverseString(reverseString(str).replace(x, y, max))

def cppTypeFromUnoTypeName(module, unoTypeName):
    primitiveTypeName = unoPrimitiveTypes.get(unoTypeName, None)
    if primitiveTypeName is not None:
        log("Finding primitive type name " + primitiveTypeName)
        t = module.FindFirstType(primitiveTypeName)
        if t.IsValid():
            log("Mapped primitive uno type " + unoTypeName + " to " + t.GetName())
            return t;

    numDots = unoTypeName.count('.')
    log('Trying to get the C++ type for ' + unoTypeName)
    for i in range(numDots + 1):
        cppTypeName = 'g::' + rreplace(unoTypeName, '.', '::', numDots - i).replace('.', '__')
        log('  Looking up C++ type ' + cppTypeName)
        t = module.FindFirstType(cppTypeName)
        if t.IsValid():
            log('  Success!')
            return t
        else:
            log('  No such C++ type')

class FirstChildSynthProvider:
    def __init__(self, valobj, internal_dict):
        log('FirstChildSynthProvider ' + valobj.GetTypeName())
        self.obj = valobj;
        while self.obj.GetNumChildren() == 1 and self.obj.GetChildAtIndex(0).GetNumChildren() == 1:
            self.obj = self.obj.GetChildAtIndex(0)
    def num_children(self):
        return self.obj.GetNumChildren()
    def get_child_index(self,name):
        return self.obj.GetIndexOfChildWithName(name)
    def get_child_at_index(self,index):
        return self.obj.GetChildAtIndex(index)
    def update(self):
        return
    def has_children(self):
        return True

class UObjectSynthProvider:
    def __init__(self, valobj, internal_dict):
        log('UObjectSynthProvider ' + valobj.GetTypeName())

        if not valobj.TypeIsPointerType():
            self.obj = valobj
            return

        address = getPtrAddress(valobj)

        frame = valobj.GetFrame()
        unoType = callMethodRaw(frame, 'uObject*', address, 'GetType()->FullName')
        if not unoType.IsValid():
            self.obj = valobj
            return
        unoTypeName = getCString(unoType)
        module = frame.GetModule()

        cppType = cppTypeFromUnoTypeName(module, unoTypeName)
        if cppType == None or not cppType.IsValid():
            self.obj = valobj
            return

        cppPointerType = cppType.GetPointerType()
        log('  The C++ (pointer) type is ' + cppPointerType.GetName())

        cppTypeName = cppType.GetName()
        typeIsStruct = frame.EvaluateExpression(cppTypeName + '_typeof()->Type == uTypeType::uTypeTypeStruct').GetValueAsSigned(0) != 0
        typeIsEnum = frame.EvaluateExpression(cppTypeName + '_typeof()->Type == uTypeType::uTypeTypeEnum').GetValueAsSigned(0) != 0
        offset = frame.EvaluateExpression('sizeof(uObject)').GetValueAsSigned(0) if typeIsStruct or typeIsEnum else 0


        log('  Address is ' + hex(address) + ' offset ' + str(offset) + ' = ' + hex(address + offset))
        replacedValobj = frame.EvaluateExpression('(' + cppPointerType.GetName() + ')' + hex(address + offset)).Dereference()
        log('  replacedValobj ' + str(replacedValobj.IsValid()))
        self.obj = replacedValobj if replacedValobj.IsValid() else valobj
    def num_children(self):
        return self.obj.GetNumChildren()
    def get_child_index(self,name):
        return self.obj.GetIndexOfChildWithName(name)
    def get_child_at_index(self,index):
        return self.obj.GetChildAtIndex(index)
    def update(self):
        return
    def has_children(self):
        return True

class UArraySynthProvider:
    def trace(self, message):
        # For debugging messages uncomment following line
        # log(message)
        pass
    def __init__(self, valobj, internal_dict):
        self.trace("UArraySynthProvider.__init__ path: \"" + valobj.path + "\" type: \"" + valobj.GetTypeName() + "\"")
        self.valobj = valobj
        self.length = None
        self.unoElementName = None
        self.cppElementType = None
        self.ptr = None
    def num_children(self):
        self.trace("UArraySynthProvider.num_children")
        result = self.length
        self.trace("UArraySynthProvider.num_children exit")
        return result
    def get_child_index(self, name):
        self.trace("UArraySynthProvider.get_child_index")
        return int(name.lstrip("[").rstrip("]"))
    def get_child_at_index(self, index):
        self.trace("UArraySynthProvider.get_child_at_index")
        self.trace("  unoElementTypeName is " + self.unoElementTypeName)
        self.trace("  cppElementType is " + self.cppElementType.GetName())
        if index >= self.length or index < 0:
            return None
        # Pointer to pointer..

        result = self.ptr.CreateChildAtOffset("[" + str(index) + "]", self.cppElementType.GetByteSize() * index, self.cppElementType)
        self.trace("UArraySynthProvider.get_child_at_index completed")
        return result
    def update(self):
        self.update_impl()
        return True
    def update_impl(self):
        self.trace("UArraySynthProvider.update")
        self.unoElementTypeName = uArrayElementTypeString(self.valobj)
        self.trace("  update unoElementTypeName is " + self.unoElementTypeName)
        cppElementType = cppTypeFromUnoTypeName(self.valobj.GetFrame().GetModule(), self.unoElementTypeName)
        if (not isValueType(self.valobj.GetFrame(), cppElementType)):
            cppElementType = cppElementType.GetPointerType()
        self.cppElementType = cppElementType
        self.trace("  update cppElementType is " + self.cppElementType.GetName())
        self.trace("  type for valobj is now " + self.valobj.GetTypeName())
        self.length = self.valobj.GetChildMemberWithName('_length').GetValueAsSigned(0)
        self.trace("  update length is " + str(self.length))
        self.ptr = self.valobj.GetChildMemberWithName("_ptr").Cast(self.cppElementType.GetPointerType())
        self.trace("UArraySynthProvider.update Element type is " + self.unoElementTypeName)
        self.trace("UArraySynthProvider.update exit")
    def has_children(self):
        self.trace("UArraySynthProvider.has_children")
        return True

def clearLog():
    # f = open(logFile, 'w')
    # f.close()
    pass

def log(str):
    # f = open(logFile, 'a')
    # f.write(str + '\n')
    # f.close()
    pass

def getCString(cstr): # hackety hax
    return cstr.summary[1:-1] # remove quotes

def getUtf16String(ptr, length):
    log('getUtf16String')
    if length == 0:
        return ''
    data = ptr.GetPointeeData(0, length)
    bytes = data.ReadRawData(lldb.SBError(), 0, 2 * length)
    str = bytes.decode('utf-16').encode('utf-8')
    log('getUtf16String result ' + str)
    return str

def getPtrAddress(value):
    return value.data.GetAddress(lldb.SBError(), 0)

def isStructType(frame, cppType):
    return frame.EvaluateExpression(cppType.GetName()  + '_typeof()->Type == uTypeType::uTypeTypeStruct').GetValueAsSigned(0) != 0

def isEnumType(frame, cppType):
    return frame.EvaluateExpression(cppType.GetName()  + '_typeof()->Type == uTypeType::uTypeTypeEnum').GetValueAsSigned(0) != 0

def isValueType(frame, cppType):
    # Definition taken from U_IS_VALUE macro in ObjectModel.h of uno
    typeType = frame.EvaluateExpression(cppType.GetName()  + '_typeof()->Type').GetValueAsSigned(0)
    typeTypeByRefConst = frame.EvaluateExpression("uTypeType::uTypeTypeByRef").GetValueAsSigned(0)
    return typeType < typeTypeByRefConst

def callMethodRaw(frame, typeName, address, methodName):
    expr = '((' + typeName + ')' + hex(address) + ')->' + methodName
    log('callMethodRaw ' + expr)
    return frame.EvaluateExpression(expr)

def callMethod(thisValue, methodName):
    return callMethodRaw(thisValue.frame, thisValue.GetTypeName(), getPtrAddress(thisValue), methodName)

def getUStringString(value):
    log('getUStringString')
    ptr = value.GetChildMemberWithName('_ptr')
    length = value.GetChildMemberWithName('_length').GetValueAsSigned(0)
    str = getUtf16String(ptr, length)
    return str

def isNull(value):
    if value.TypeIsPointerType():
        address = value.GetData().GetAddress(lldb.SBError(), 0)
        return address == 0
    else:
        return False

def uObjectToString(value):
    log('uObjectToString')
    if isNull(value):
        return 'null'
    ustring = None
    if value.TypeIsPointerType():
        log('Object?')
        ustring = callMethod(value, 'ToString()')
    else:
        x = 'uBoxPtr(%s_typeof(), (void*)%s, nullptr, false)->ToString()' % (value.GetTypeName(), hex(getPtrAddress(value.AddressOf())))
        log('Struct? ' + x)
        ustring = value.frame.EvaluateExpression(x)
    string = getUStringString(ustring)
    log('uObjectToString result ' + string)
    return string

def uArrayElementTypeString(value):
    log('uArrayElementType ' + value.GetTypeName())
    elementTypeUString = value.frame.EvaluateExpression(
        '((uArrayType*)((uArray*)%s)->GetType())->ElementType->ToString()' % hex(getPtrAddress(value)))
    return getUStringString(elementTypeUString)

def uStringSummary(value, *rest):
    log('- uStringSummary')
    if isNull(value):
        return 'null'
    return '"%s"' % getUStringString(value)

def uObjectSummary(value, *rest):
    log('- uObjectSummary ' + value.GetTypeName())
    return uObjectToString(value)

def uTypeSummary(value, *rest):
    log('- uTypeSummary ' + value.GetTypeName())
    return uObjectToString(value)

def uArraySummary(value, *rest):
    log(' - uArraySummary')
    if isNull(value):
        return 'null'
    length = callMethod(value, 'Length()').GetValueAsSigned(0)
    log('Length ' + str(length))
    return uArrayElementTypeString(value) + "[" + str(length) + "]"

def firstChildSummary(value, *rest):
    log('- firstChildSummary ' + value.GetTypeName())
    log(str(value.GetNumChildren()))
    while value.GetNumChildren() == 1:
        value = value.GetChildAtIndex(0)
    return value.GetSummary()

def __lldb_init_module(debugger, dict):
    clearLog()
    log('********************************** init module 3')
    category = debugger.GetDefaultCategory()
    for type in ['uString', 'uObject', 'uType', 'uArray']:
        summary = lldb.SBTypeSummary.CreateWithFunctionName(moduleName + '.' + type + 'Summary')
        isRegex = False
        debugger.GetDefaultCategory().AddTypeSummary(lldb.SBTypeNameSpecifier(type, isRegex), summary)
    for type in ['uStrong', 'uSStrong', 'uWeak', 'uSWeak']:
        summary = lldb.SBTypeSummary.CreateWithFunctionName(moduleName + '.firstChildSummary')
        isRegex = True
        category.AddTypeSummary(lldb.SBTypeNameSpecifier(type + '<.*>', isRegex), summary)

        synthetic = lldb.SBTypeSynthetic.CreateWithClassName(moduleName + '.FirstChildSynthProvider')
        synthetic.SetOptions(lldb.eTypeOptionCascade)
        category.AddTypeSynthetic(lldb.SBTypeNameSpecifier(type + '<.*>', isRegex), synthetic)
    summary = lldb.SBTypeSummary.CreateWithFunctionName(moduleName + '.uObjectSummary')
    isRegex = True
    category.AddTypeSummary(lldb.SBTypeNameSpecifier('g::.+', isRegex), summary)

    synthetic = lldb.SBTypeSynthetic.CreateWithClassName(moduleName + '.UObjectSynthProvider')
    synthetic.SetOptions(lldb.eTypeOptionCascade)
    category.AddTypeSynthetic(lldb.SBTypeNameSpecifier('uObject', False), synthetic)

    synthetic = lldb.SBTypeSynthetic.CreateWithClassName(moduleName + '.UObjectSynthProvider')
    synthetic.SetOptions(lldb.eTypeOptionCascade)
    category.AddTypeSynthetic(lldb.SBTypeNameSpecifier('g::.+', True), synthetic)

    synthetic = lldb.SBTypeSynthetic.CreateWithClassName(moduleName + '.UArraySynthProvider')
    synthetic.SetOptions(lldb.eTypeOptionCascade)
    category.AddTypeSynthetic(lldb.SBTypeNameSpecifier('uArray', False), synthetic)

