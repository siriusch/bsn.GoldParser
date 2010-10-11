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
using System.Globalization;
using System.IO;

using bsn.GoldParser.Grammar;
using bsn.GoldParser.Parser;
using bsn.GoldParser.Semantic;

namespace bsn.GoldParser.Sample {
	internal class Program {
		private static void Main(string[] args) {
			Console.WriteLine("*** CALCULATOR SAMPLE *** (input formula, empty line terminates)");
			CompiledGrammar grammar = CompiledGrammar.Load(typeof(CalculatorToken), "Calculator.cgt");
			SemanticTypeActions<CalculatorToken> actions = new SemanticTypeActions<CalculatorToken>(grammar);
			try {
				actions.Initialize(true);
			} catch (InvalidOperationException ex) {
				Console.Write(ex.Message);
				Console.ReadKey(true);
				return;
			}
			for (string formula = Console.ReadLine(); !string.IsNullOrEmpty(formula); formula = Console.ReadLine()) {
				SemanticProcessor<CalculatorToken> processor = new SemanticProcessor<CalculatorToken>(new StringReader(formula), actions);
				ParseMessage parseMessage = processor.ParseAll();
				if (parseMessage == ParseMessage.Accept) {
					Console.WriteLine(string.Format(NumberFormatInfo.InvariantInfo, "Result: {0}", ((Computable)processor.CurrentToken).GetValue()));
				} else {
					IToken token = processor.CurrentToken;
					Console.WriteLine(string.Format("{0} {1}", "^".PadLeft(token.Position.Index+1), parseMessage));
				}
			}
		}
	}
}
