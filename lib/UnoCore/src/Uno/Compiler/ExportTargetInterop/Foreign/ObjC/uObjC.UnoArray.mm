#include <Uno/Uno.h>
#include <uObjC.Foreign.h>
#include <uObjC.UnoArray.h>

@implementation StrongUnoArray
{
    @private uArray* _unoArray;
}

@synthesize getAt = _getAt;
@synthesize setAt = _setAt;

+ (instancetype)strongUnoArrayWithUnoArray:(uArray*)array
	getAt:(id (^) (uArray*, int))get
	setAt:(void (^) (uArray*, int, id))set
{
	if (array == nullptr)
	{
		return nil;
	}
	return [[StrongUnoArray alloc]
		initWithUnoArray: array
		getAt: get
		setAt: set];
}

- (id)initWithUnoArray:(uArray*)array
	getAt:(id (^) (uArray*, int))get
	setAt:(void (^) (uArray*, int, id))set
{
	self = [super init];

	if (self)
	{
		uRetain(array);
		_unoArray = array;
		self.getAt = get;
		self.setAt = set;
	}

	return self;
}

- (id)init
{
	return [self initWithUnoArray: nullptr getAt: nil setAt: nil];
}

- (instancetype)copyWithZone:(NSZone*)zone
{
	uRetain(_unoArray);
	return [[[self class] allocWithZone: zone] initWithUnoArray: _unoArray getAt: self.getAt setAt: self.setAt];
}

- (uArray*)unoArray
{
	uRetain(_unoArray);
	uAutoRelease(_unoArray);

	return _unoArray;
}

- (void)setUnoArray:(uArray*)arr
{
	uRetain(arr);
	uAutoRelease(_unoArray);
	_unoArray = arr;
}

- (id)objectAtIndexedSubscript:(int)idx
{
	uForeignPool autoReleasePool;
	return self.getAt(_unoArray, idx);
}

- (void)setObject:(id)obj atIndexedSubscript:(int)idx;
{
	uForeignPool autoReleasePool;
	self.setAt(_unoArray, idx, obj);
}

- (NSUInteger)count
{
	uForeignPool autoReleasePool;
	return (NSUInteger)@{byte[]:Of(_unoArray).Length:Get()};
}

- (void)dealloc
{
	uRelease(_unoArray);
	_unoArray = nullptr;
	self.getAt = nil;
	self.setAt = nil;
}

- (NSArray*)copyArray
{
	uForeignPool autoReleasePool;
	int len = (int)[self count];
	NSMutableArray* result = [NSMutableArray arrayWithCapacity: len];
	for (int i = 0; i < len; ++i)
	{
		[result addObject: self.getAt(_unoArray, i)];
	}
	return result;
}

@end
