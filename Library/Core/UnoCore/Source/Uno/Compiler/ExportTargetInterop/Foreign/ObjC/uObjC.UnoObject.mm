#include <Uno/Uno.h>
#include <uObjC.Foreign.h>
#include <uObjC.UnoObject.h>


@implementation StrongUnoObject
{
	@private
	uObject* _unoObject;
}

+ (instancetype)strongUnoObjectWithUnoObject:(uObject*)object
{
	if (object == NULL)
	{
		return nil;
	}
	return [[StrongUnoObject alloc] initWithUnoObject: object];
}

- (id)initWithUnoObject:(uObject *)object
{
	self = [super init];
	
	if (self)
	{
		uRetain(object);
		_unoObject = object;
	}
	
	return self;
}

- (id)init
{
	return [self initWithUnoObject: NULL];
}

- (instancetype)copyWithZone:(NSZone *)zone
{
	StrongUnoObject *newRef = [[[self class] allocWithZone:zone] init];

	uRetain(_unoObject);
	newRef->_unoObject = _unoObject;
	return newRef;
}

- (uObject *)unoObject
{
	// Auto-released

	uRetain(_unoObject);
	uAutoRelease(_unoObject);

	return _unoObject;
}

- (void)setUnoObject:(uObject *)object
{
	uRetain(object);
	uAutoRelease(_unoObject);

	_unoObject = object;
}

- (void)dealloc
{
	uRelease(_unoObject);
	_unoObject = NULL;
}
@end


@implementation WeakUnoObject
{
	@private
	uWeakObject *_weakObject;
}

+ (instancetype)weakUnoObjectWithUnoObject:(uObject*)object
{
	if (object == NULL)
	{
		return nil;
	}
	return [[WeakUnoObject alloc] initWithUnoObject: object];
}

- (id)initWithUnoObject:(uObject *)object
{
	self = [super init];
	if (self)
	{
		_weakObject = NULL;
		uStoreWeak(&_weakObject, object);
	}
	return self;
}

- (id)init
{
	return [self initWithUnoObject: NULL];
}

- (instancetype)copyWithZone:(NSZone *)zone
{
	WeakUnoObject *newRef = [[[self class] allocWithZone:zone] init];

	newRef->_weakObject = NULL;

	uObject *object = uLoadWeak(_weakObject);
	uStoreWeak(&newRef->_weakObject, object);

	return newRef;
}

- (uObject *)unoObject
{
	// Auto-released
	return uLoadWeak(_weakObject);
}

- (void)setUnoObject:(uObject *)object
{
	uStoreWeak(&_weakObject, object);
}

- (void)dealloc
{
	uStoreWeak(&_weakObject, NULL);
}
@end


@implementation UnsafeUnoObject

@synthesize unoObject = _unoObject;

+ (instancetype)unsafeUnoObjectWithUnoObject:(uObject*)object
{
	if (object == NULL)
	{
		return nil;
	}
	return [[UnsafeUnoObject alloc] initWithUnoObject: object];
}

- (id)initWithUnoObject:(uObject *)object
{
	self = [super init];
	if (self)
	{
		_unoObject = object;
	}
	return self;
}

- (id)init
{
	return [self initWithUnoObject: NULL];
}

- (instancetype)copyWithZone:(NSZone *)zone
{
	UnsafeUnoObject *newRef = [[[self class] allocWithZone:zone] init];
	newRef->_unoObject = _unoObject;
	return newRef;
}
@end
