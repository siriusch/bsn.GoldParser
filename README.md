bsn GoldParser .NET Engine
==========================

Copyright 2009-2016 by Arsène von Wyss - arsene.vonwyss@sirius.ch

Development has been supported by Sirius Technologies AG, Basel

What is it?
-----------

The engine can consume EGT (and the older CGT) compiled grammar files produced by [Devin Cook's GOLD Parsing System](http://goldparser.org/). This engine is therefore not a standalone parsing solution.

However, it supports all features of GOLD and has the unique ability to directly create specific AST node instances while parsing instead of just creating a parse tree. The bsn.Goldparser.Sample project contains a full working sample, and there are some CodeProject articles such as [The Whole Shebang: Building Your Own General Purpose Language Interpreter](https://www.codeproject.com/Articles/129965/The-Whole-Shebang-Building-your-own-general-purpos) which explain the whole process.

Source
------

[https://github.com/siriusch/bsn.GoldParser](https://github.com/siriusch/bsn.GoldParser)

License
-------

The library is distributed under the GNU Lesser General Public License:
http://www.gnu.org/licenses/lgpl.html

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
