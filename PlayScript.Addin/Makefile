CONFIG?=Debug

all:
	xbuild /p:Configuration=${CONFIG} ${ARGS}

clean:
	xbuild /t:Clean ${ARGS}

# Clean does not always really clean, so clean hard ;-)
cleanhard:
	find . -name "obj" -print0 | xargs -0 -n 1 rm -Rf
	find . -name "bin" -print0 | xargs -0 -n 1 rm -Rf

install:
	xbuild /p:InstallAddin=True /p:Configuration=${CONFIG} ${ARGS}
	-find . -name "*.mpack"
