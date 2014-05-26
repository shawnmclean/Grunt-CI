
echo test

cd %1

Rem Remove readonly attribute, this will be usefull on grunt for any file write/delete/modify operations
attrib -r *.* /s

rem npm install
