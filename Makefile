.PHONY: lib release diagrams
default:
	@bash scripts/build.sh
lib:
	@bin/uno doctor -e lib
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
