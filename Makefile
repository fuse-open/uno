.PHONY: lib release

default:
	@bash scripts/build.sh
lib:
	@bin/uno doctor -e lib
unocore:
	@bin/uno build lib/UnoCore
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
