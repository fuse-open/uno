.PHONY: release diagrams
default:
	@bash scripts/build.sh
unocore:
	@bash scripts/build.sh --unocore
rebuild:
	@bash scripts/build.sh --rebuild
all:
	@bash scripts/build.sh --all
release:
	@bash scripts/pack.sh
diagrams:
	@bash scripts/build-diagrams.sh
check:
	@bash scripts/test.sh
clean:
	@bash scripts/clean.sh
install:
	@ln -sfv "`pwd -P`"/prebuilt/{uno,uno-dev} /usr/local/bin
