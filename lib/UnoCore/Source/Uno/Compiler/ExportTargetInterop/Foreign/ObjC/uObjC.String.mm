#include "uObjC.String.h"

#include <Foundation/NSString.h>

namespace uObjC
{
	#if __BIG_ENDIAN__
	    enum { NativeUTF16Encoding = NSUTF16BigEndianStringEncoding };
	#else
	    enum { NativeUTF16Encoding = NSUTF16LittleEndianStringEncoding };
	#endif

	uString* UnoString(NSString* string)
	{
		if (!string)
		{
			return nullptr;
		}
		
		NSUInteger bytes = [string
			lengthOfBytesUsingEncoding: NativeUTF16Encoding];
		
		uString* result = uString::New(bytes / sizeof(char16_t));
		
		NSUInteger usedBytes = 0;
		if ([string
			getBytes: result->_ptr
			maxLength: bytes
			usedLength: &usedBytes
			encoding: NativeUTF16Encoding
			options: 0
			range: NSMakeRange(0, [string length])
			remainingRange: nullptr])
		{
			if (usedBytes != bytes)
			{
				result->_length = usedBytes / sizeof(char16_t);
				result->_ptr[result->_length] = 0;
			}
			return result;
		}
	
		return nullptr;
	}
	
	NSString* NativeString(uString* string)
	{
		if (!string)
		{
			return nullptr;
		}
	
		return [[NSString alloc]
				initWithBytes: string->Ptr()
				length: string->Length() * sizeof(char16_t)
				encoding: NativeUTF16Encoding];
	}
}
