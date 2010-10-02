// bsn GoldParser .NET Engine
// --------------------------
// 
// Copyright 2009, 2010 by Arsène von Wyss - avw@gmx.ch
// 
// Development has been supported by Sirius Technologies AG, Basel
// 
// Source:
// 
// https://bsn-goldparser.googlecode.com/hg/
// 
// License:
// 
// The library is distributed under the GNU Lesser General Public License:
// http://www.gnu.org/licenses/lgpl.html
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using bsn.GoldParser.Grammar;

namespace PackCgt {
	public class Program {
		static void Main(string[] args) {
			Console.WriteLine("bsn GoldParser CGT packer");
			Console.WriteLine("-------------------------");
			Console.WriteLine("(C) 2010 Arsène von Wyss");
			Console.WriteLine();
			if (args.Length == 0) {
				Console.WriteLine("Usage: PackCgt cgtfile");
				Environment.Exit(1);
			} 
			try {
				using (FileStream file = new FileStream(args[0], FileMode.Open, FileAccess.ReadWrite, FileShare.Read)) {
					using (MemoryStream packed = new MemoryStream()) {
						CompiledGrammar.Pack(file, packed);
						if (file.Length <= packed.Length) {
							Console.WriteLine("The file size could not be reduced more");
						} else {
							file.Seek(0, SeekOrigin.Begin);
							file.Write(packed.GetBuffer(), 0, (int)packed.Length);
							Console.WriteLine("Reduced file from {0} bytes to {1} bytes", file.Length, packed.Length);
							file.SetLength(packed.Length);
						}
					}
				}
			} catch (Exception ex) {
				Debug.WriteLine(ex, "Error while packing CGT");
				Console.WriteLine("Error: "+ex.Message);
				Environment.Exit(1);
			}
		}
	}
}
