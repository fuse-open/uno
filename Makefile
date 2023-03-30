.PHONY: lib release

default:
	@bash scripts/build.sh
lib:
	@bin/net6.0/uno doctor -e lib
unocore:
	@bin/net6.0/uno build lib/UnoCore -DLIBRARY
uno:
	@dotnet build uno.sln -v m
disasm:
	@dotnet build disasm.sln -v m
runtime:
	@dotnet build runtime.sln -v m
nupkg:
	@bash scripts/nupkg.sh
release:
	@bash scripts/pack.sh
check:
	@bash scripts/test.sh dotnet
clean:
	@bash scripts/clean.sh
install:
	@npm link -f
