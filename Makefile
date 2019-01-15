.PHONY: release diagrams
default:
	@bash scripts/build.sh
unocore:
	@bash scripts/build.sh --unocore
release:
	@bash scripts/pack.sh
diagrams:
	@bash scripts/build-diagrams.sh
check:
	@bash scripts/test.sh
clean:
	@bash scripts/clean.sh
install:
	@ln -sfv "`pwd -P`"/bin/uno /usr/local/bin
