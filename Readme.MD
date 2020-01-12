# Как добавить
Добавить в External Tools в VS. 
Указав любое удобное название, например ```Copy all to GAC```.
Команда - путь до исполняемого файла, например ```C:\tools\gacify.exe```
Аргументы - ```$(SolutionDir)``` или ```$(SolutionDir) "C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\gacutil.exe"```,
где второй аргумент - путь до gacutil.
Поставить чекбокс - ```Использовать окно вывода```

# Как использовать
1. Билдим решение 
2. External Tools -> Copy all to GAC
3. Смотрим вывод

# Статус
[![Build Status](https://travis-ci.com/Rombique/gacify.svg?branch=master)](https://travis-ci.com/Rombique/gacify)

[Download](https://github.com/Rombique/gacify/raw/master/gacify.zip)
