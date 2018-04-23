#!/bin/bash
SELF=`echo $0 | sed 's/\\\\/\\//g'`
cd "`dirname "$SELF"`/.." || exit 1

if [ "$OSTYPE" = msys ]; then
    PATH="$PROGRAMFILES (x86)/Graphviz2.38/bin:$PROGRAMFILES/Graphviz2.38/bin:$PATH"
fi

mkdir -p diagrams

for f in `find src -name "*.dot"`; do
    out=`basename "$f" | sed 's/.dot/.png/'`
    dot $f -Tpng -o"diagrams/${out}"
done
