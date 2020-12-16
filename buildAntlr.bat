pushd %~dp0

pushd Projects\JavaDocParser\Parser
java -jar ..\..\..\Lib\antlr-4.8-complete.jar -o .\Antlr -package Eir.MFSH.Parser -no-listener -visitor -Dlanguage=CSharp JavadocLexer.g4
java -jar ..\..\..\Lib\antlr-4.8-complete.jar -o .\Antlr -package Eir.MFSH.Parser -no-listener -visitor -Dlanguage=CSharp JavadocParser.g4
popd

popd

pause
