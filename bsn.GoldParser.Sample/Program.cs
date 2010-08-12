using System;
using System.Globalization;
using System.IO;
using System.Linq;

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
				Console.WriteLine("Failed to initialize:");
				Console.WriteLine(ex.Message);
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
