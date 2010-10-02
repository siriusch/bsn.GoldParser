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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// Contains grammar tables required for parsing.
	/// </summary>
	/// <remarks>The code is inspired by the GOLD C# engine by Vladimir Morozov (vmoroz@hotmail.com)</remarks>
	public class CompiledGrammar {
		/// <summary>
		/// Edge between DFA states.
		/// </summary>
		internal struct DfaEdge {
			public readonly int CharSetIndex;
			public readonly int TargetIndex;

			internal DfaEdge(int charSetIndex, int targetIndex) {
				CharSetIndex = charSetIndex;
				TargetIndex = targetIndex;
			}
		}

		private class LoadContext: CgtReader {
			private int dfaInitialStateIndex; // DFA initial state index
			private int lrInitialState; // LR initial state
			private int startSymbolIndex; // Start symbol index

			public LoadContext(BinaryReader reader): base(reader) {}

			public int DfaInitialStateIndex {
				get {
					return dfaInitialStateIndex;
				}
				set {
					dfaInitialStateIndex = value;
				}
			}

			public int LrInitialState {
				get {
					return lrInitialState;
				}
				set {
					lrInitialState = value;
				}
			}

			public int StartSymbolIndex {
				get {
					return startSymbolIndex;
				}
				set {
					startSymbolIndex = value;
				}
			}
		}

		/// <summary>
		/// Identifies Gold parser grammar file.
		/// </summary>
		public const string FileHeader = "GOLD Parser Tables/v1.0";

		public static bool CanContinueParsing(ParseMessage message) {
			switch (message) {
			case ParseMessage.CommentBlockRead:
			case ParseMessage.CommentLineRead:
			case ParseMessage.Reduction:
			case ParseMessage.TokenRead:
				return true;
			}
			return false;
		}

		public static void Pack(Stream input, Stream output) {
			Pack(input, output, false);
		}

		public static void Pack(Stream input, Stream output, bool gzip) {
			if (gzip) {
				using (MemoryStream buffer = new MemoryStream((input.CanSeek) ? (int)input.Length : 512000)) {
					Pack(new BinaryReader(input), new BinaryWriter(buffer));
					using (MemoryStream bufferGZip = new MemoryStream((int)buffer.Length)) {
						using (GZipStream compressedStream = new GZipStream(bufferGZip, CompressionMode.Compress, true)) {
							compressedStream.Write(buffer.GetBuffer(), 0, (int)buffer.Length);
						}
						// due to hardcoded huffman trees in the .NET source code, the GZip'ed stream may be MUCH longer than the original, so we check that and take the shorter one
						// see also: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=93636
						MemoryStream shortestBuffer = (buffer.Length <= bufferGZip.Length) ? buffer : bufferGZip;
						output.Write(shortestBuffer.GetBuffer(), 0, (int)shortestBuffer.Length);
					}
				}
			} else {
				Pack(new BinaryReader(input), new BinaryWriter(output));
			}
		}

		public static void Pack(BinaryReader input, BinaryWriter output) {
			if (input == null) {
				throw new ArgumentNullException("input");
			}
			if (output == null) {
				throw new ArgumentNullException("output");
			}
			CgtReader reader = new CgtReader(input);
			CgtWriter writer = new CgtWriter(output);
			string header = reader.ReadHeaderString();
			if (FileHeader != header) {
				throw new FileLoadException("The File Header is invalid");
			}
			writer.WriteHeaderString(header);
			while (reader.HasMoreData()) {
				CgtRecordType recordType = reader.ReadNextRecord();
				switch (recordType) {
				case CgtRecordType.Charsets:
					int index = reader.ReadIntegerEntry();
					string chars = reader.ReadStringEntry();
					DfaCharset charset = new DfaCharset(null, index, chars);
					if (charset.SequenceLength >= 4) {
						writer.WriteNextRecord(CgtRecordType.PackedCharsets, 4);
						writer.WriteIntegerEntry(index);
						writer.WriteIntegerEntry(charset.SequenceStart);
						writer.WriteIntegerEntry(charset.SequenceEnd);
						writer.WriteStringEntry(new string(charset.CharactersExcludingSequence));
					} else {
						writer.WriteNextRecord(CgtRecordType.Charsets, 2);
						writer.WriteIntegerEntry(index);
						writer.WriteStringEntry(chars);
					}
					break;
				default:
					writer.WriteNextRecord(recordType, reader.EntryCount);
					while (reader.EntryCount > 0) {
						reader.CopyEntry(writer);
					}
					break;
				}
			}
			output.Flush();
		}

		/// <summary>
		/// Loads the CompiledGrammar from the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public static CompiledGrammar Load(Stream stream) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			if (stream.CanSeek) {
				long position = stream.Position;
				try {
					using (GZipStream packedStream = new GZipStream(stream, CompressionMode.Decompress, true)) {
						return Load(new BinaryReader(packedStream));
					}
				} catch (InvalidDataException) {
				} catch (FileLoadException) {
					// not a compressed stream
				}
				stream.Seek(position, SeekOrigin.Begin);
			}
			return Load(new BinaryReader(stream));
		}

		/// <summary>
		/// Loads the CompiledGrammar from the specified reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public static CompiledGrammar Load(BinaryReader reader) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			return new CompiledGrammar(reader);
		}

		/// <summary>
		/// Loads the CompiledGrammar from the specified manifest resource of the <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="name">The name.</param>
		/// <seealso cref="Assembly.GetManifestResourceStream(Type,string)"/>
		/// <returns></returns>
		public static CompiledGrammar Load(Type type, string name) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			using (Stream stream = type.Assembly.GetManifestResourceStream(type, name)) {
				return Load(stream);
			}
		}

		private readonly object sync = new object();
		private string about; // Grammar description
		private string author; // Author of the grammar
		private ICollection<DfaState> blockCommentStates;
		private bool caseSensitive; // Grammar is case sensitive or not
		private DfaCharset[] charSetTable; // Charset table
		private DfaState dfaInitialState; // DFA initial state 
		private DfaState[] dfaStateTable; // DFA state table
		private Symbol endSymbol;
		private Symbol errorSymbol;
		private LalrState lalrInitialState; // LR state table
		private LalrState[] lalrStateTable; // LR state table
		private string name; // Name of the grammar
		private Rule[] ruleTable; // Rule table
		private Dictionary<Symbol, ReadOnlyCollection<Rule>> rules;
		private Symbol startSymbol;
		private Dictionary<DfaState, ReadOnlyCollection<DfaState>> stateOrigins;
		private Symbol[] symbolTable; // Symbol table
		private Dictionary<string, Symbol> symbols;
		private string version; // Version of the grammar

		/// <summary>
		/// Creates a new instance of <c>CompiledGrammar</c> class
		/// </summary>
		/// <param name="reader"></param>
		protected CompiledGrammar(BinaryReader reader) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			Load(new LoadContext(reader));
		}

		/// <summary>
		/// Gets grammar description.
		/// </summary>
		public string About {
			get {
				return about;
			}
		}

		/// <summary>
		/// Gets grammar author.
		/// </summary>
		public string Author {
			get {
				return author;
			}
		}

		public ICollection<DfaState> BlockCommentStates {
			get {
				return blockCommentStates;
			}
		}

		/// <summary>
		/// Gets the value indicating if the grammar is case sensitive.
		/// </summary>
		public bool CaseSensitive {
			get {
				return caseSensitive;
			}
		}

		/// <summary>
		/// Gets the dfa charset count.
		/// </summary>
		/// <value>The dfa charset count.</value>
		public int DfaCharsetCount {
			get {
				return charSetTable.Length;
			}
		}

		/// <summary>
		/// Gets initial DFA state.
		/// </summary>
		public DfaState DfaInitialState {
			get {
				return dfaInitialState;
			}
		}

		/// <summary>
		/// Gets the dfa state count.
		/// </summary>
		/// <value>The dfa state count.</value>
		public int DfaStateCount {
			get {
				return dfaStateTable.Length;
			}
		}

		/// <summary>
		/// Gets a special symbol to designate last token in the input stream.
		/// </summary>
		public Symbol EndSymbol {
			get {
				return endSymbol;
			}
		}

		/// <summary>
		/// Gets the error symbol.
		/// </summary>
		/// <value>The error symbol.</value>
		public Symbol ErrorSymbol {
			get {
				return errorSymbol;
			}
		}

		/// <summary>
		/// Gets initial LR state.
		/// </summary>
		public LalrState InitialLRState {
			get {
				return lalrInitialState;
			}
		}

		/// <summary>
		/// Gets the lalr state count.
		/// </summary>
		/// <value>The lalr state count.</value>
		public int LalrStateCount {
			get {
				return lalrStateTable.Length;
			}
		}

		/// <summary>
		/// Gets grammar name.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// Gets the rule count.
		/// </summary>
		/// <value>The rule count.</value>
		public int RuleCount {
			get {
				return ruleTable.Length;
			}
		}

		/// <summary>
		/// Gets the start symbol for the grammar.
		/// </summary>
		public Symbol StartSymbol {
			get {
				return startSymbol;
			}
		}

		/// <summary>
		/// Gets the symbol count.
		/// </summary>
		/// <value>The symbol count.</value>
		public int SymbolCount {
			get {
				return symbolTable.Length;
			}
		}

		/// <summary>
		/// Gets grammar version.
		/// </summary>
		public string Version {
			get {
				return version;
			}
		}

		/// <summary>
		/// Gets the dfa charset.
		/// </summary>
		/// <param name="charSetIndex">Index of the char set.</param>
		/// <returns></returns>
		public DfaCharset GetDfaCharset(int charSetIndex) {
			return charSetTable[charSetIndex];
		}

		/// <summary>
		/// Gets the state of the dfa.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public DfaState GetDfaState(int index) {
			return dfaStateTable[index];
		}

		public ICollection<DfaState> GetDfaStatesOfSymbols(Predicate<Symbol> filter) {
			int symbolsFound = 0;
			SymbolSet acceptSymbols = new SymbolSet();
			for (int i = 0; i < SymbolCount; i++) {
				Symbol symbol = GetSymbol(i);
				switch (symbol.Kind) {
				case SymbolKind.CommentStart:
				case SymbolKind.CommentEnd:
					acceptSymbols[symbol] = true;
					symbolsFound++;
					break;
				}
			}
			if (symbolsFound != 0) {
				Queue<DfaState> stateQueue = new Queue<DfaState>();
				for (int i = 0; i < DfaStateCount; i++) {
					DfaState state = GetDfaState(i);
					if (acceptSymbols[state.AcceptSymbol]) {
						stateQueue.Enqueue(state);
					}
				}
				Debug.Assert(stateQueue.Count >= symbolsFound);
				Dictionary<DfaState, int> allowedStates = new Dictionary<DfaState, int>();
				do {
					DfaState state = stateQueue.Dequeue();
					int count;
					if (!allowedStates.TryGetValue(state, out count)) {
						foreach (DfaState originState in state.GetOriginStates()) {
							stateQueue.Enqueue(originState);
						}
					}
					allowedStates[state] = count+1;
				} while (stateQueue.Count > 0);
				return allowedStates.Keys;
			}
			return new DfaState[0];
		}

		/// <summary>
		/// Gets the state of the lalr.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public LalrState GetLalrState(int index) {
			return lalrStateTable[index];
		}

		/// <summary>
		/// Gets the rule.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public Rule GetRule(int index) {
			return ruleTable[index];
		}

		/// <summary>
		/// Gets the rules for a nonterminal symbol.
		/// </summary>
		/// <param name="symbol">The nonterminal symbol.</param>
		/// <returns>A collection of associated rules.</returns>
		public ReadOnlyCollection<Rule> GetRulesForSymbol(Symbol symbol) {
			ReadOnlyCollection<Rule> result;
			if (!TryGetRulesForSymbol(symbol, out result)) {
				throw new ArgumentException("No rules exist for the given symbol", "symbol");
			}
			return result;
		}

		/// <summary>
		/// Gets the symbol.
		/// </summary>
		/// <param name="symbolIndex">Index of the symbol.</param>
		/// <returns></returns>
		public Symbol GetSymbol(int symbolIndex) {
			return symbolTable[symbolIndex];
		}

		/// <summary>
		/// Gets the symbol with the specified name.
		/// </summary>
		/// <param name="symbolName">Name of the symbol (including <c>&lt; &gt;</c> for non-terminals) .</param>
		/// <returns>The matching symbol.</returns>
		/// <exception cref="ArgumentException">The symbol name was not found.</exception>
		public Symbol GetSymbolByName(string symbolName) {
			Symbol result;
			if (!TryGetSymbol(symbolName, out result)) {
				throw new ArgumentException("No symbol exists with the given name", "symbolName");
			}
			return result;
		}

		public override string ToString() {
			return Name;
		}

		/// <summary>
		/// Tries the get rules for symbol.
		/// </summary>
		/// <param name="symbol">The symbol.</param>
		/// <param name="rules">The rules.</param>
		/// <returns></returns>
		public bool TryGetRulesForSymbol(Symbol symbol, out ReadOnlyCollection<Rule> rules) {
			if (symbol == null) {
				throw new ArgumentNullException("symbol");
			}
			if (symbol.Owner != this) {
				throw new ArgumentException("The symbol belongs to another grammar");
			}
			if (symbol.Kind != SymbolKind.Nonterminal) {
				rules = null;
				return false;
			}
			InitializeRuleLookup();
			if (!this.rules.TryGetValue(symbol, out rules)) {
				rules = Array.AsReadOnly(new Rule[0]);
			}
			return true;
		}

		/// <summary>
		/// Tries the get symbol.
		/// </summary>
		/// <param name="symbolName">Name of the symbol.</param>
		/// <param name="symbol">The symbol.</param>
		/// <returns></returns>
		public bool TryGetSymbol(string symbolName, out Symbol symbol) {
			InitializeSymbolLookup();
			return symbols.TryGetValue(symbolName, out symbol);
		}

		/// <summary>
		/// Gets the state transition origin states.
		/// </summary>
		/// <param name="state">The state to get the transitions origin vectors for.</param>
		/// <returns></returns>
		internal ReadOnlyCollection<DfaState> GetStateOrigins(DfaState state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			InitializeStateOriginLookup();
			ReadOnlyCollection<DfaState> result;
			if (!stateOrigins.TryGetValue(state, out result)) {
				throw new ArgumentException("The state is not valid", "state");
			}
			return result;
		}

		private void InitializeRuleLookup() {
			lock (sync) {
				if (rules == null) {
					Dictionary<Symbol, List<Rule>> ruleTemp = new Dictionary<Symbol, List<Rule>>();
					foreach (Rule rule in ruleTable) {
						List<Rule> ruleList;
						if (!ruleTemp.TryGetValue(rule.RuleSymbol, out ruleList)) {
							ruleList = new List<Rule>();
							ruleTemp.Add(rule.RuleSymbol, ruleList);
						}
						ruleList.Add(rule);
					}
					rules = new Dictionary<Symbol, ReadOnlyCollection<Rule>>(ruleTemp.Count);
					foreach (KeyValuePair<Symbol, List<Rule>> pair in ruleTemp) {
						rules.Add(pair.Key, Array.AsReadOnly(pair.Value.ToArray()));
					}
				}
			}
		}

		private void InitializeStateOriginLookup() {
			lock (sync) {
				if (stateOrigins == null) {
					Dictionary<DfaState, List<DfaState>> stateOriginTemp = new Dictionary<DfaState, List<DfaState>>(dfaStateTable.Length);
					foreach (DfaState state in dfaStateTable) {
						foreach (DfaState transitionState in state.GetTransitionStates()) {
							List<DfaState> originList;
							if (!stateOriginTemp.TryGetValue(transitionState, out originList)) {
								originList = new List<DfaState>();
								stateOriginTemp.Add(transitionState, originList);
							}
							originList.Add(state);
						}
					}
					stateOrigins = new Dictionary<DfaState, ReadOnlyCollection<DfaState>>(dfaStateTable.Length);
					foreach (DfaState state in dfaStateTable) {
						DfaState[] origins;
						List<DfaState> originList;
						if (stateOriginTemp.TryGetValue(state, out originList)) {
							origins = originList.ToArray();
						} else {
							origins = new DfaState[0];
						}
						stateOrigins.Add(state, Array.AsReadOnly(origins));
					}
				}
			}
		}

		private void InitializeSymbolLookup() {
			lock (sync) {
				if (symbols == null) {
					symbols = new Dictionary<string, Symbol>(StringComparer.Ordinal);
					foreach (Symbol symbol in symbolTable) {
						symbols.Add(symbol.ToString(), symbol);
					}
				}
			}
		}

		/// <summary>
		/// Loads grammar from the binary reader.
		/// </summary>
		private void Load(LoadContext context) {
			if (FileHeader != context.ReadHeaderString()) {
				throw new FileLoadException("The File Header is invalid");
			}
			while (context.HasMoreData()) {
				CgtRecordType recordType = context.ReadNextRecord();
				switch (recordType) {
				case CgtRecordType.Parameters:
					ReadHeader(context);
					break;
				case CgtRecordType.TableCounts:
					ReadTableCounts(context);
					break;
				case CgtRecordType.Initial:
					ReadInitialStates(context);
					break;
				case CgtRecordType.Symbols:
					ReadSymbols(context);
					break;
				case CgtRecordType.Charsets:
					ReadCharsets(context);
					break;
				case CgtRecordType.PackedCharsets:
					ReadPackedCharsets(context);
					break;
				case CgtRecordType.Rules:
					ReadRules(context);
					break;
				case CgtRecordType.DfaStates:
					ReadDfaStates(context);
					break;
				case CgtRecordType.LRStates:
					ReadLRStates(context);
					break;
				default:
					throw new FileLoadException("Invalid record count");
				}
			}
			dfaInitialState = dfaStateTable[context.DfaInitialStateIndex];
			startSymbol = symbolTable[context.StartSymbolIndex];
			lalrInitialState = lalrStateTable[context.LrInitialState];
			blockCommentStates = GetDfaStatesOfSymbols(symbol => (symbol.Kind == SymbolKind.CommentStart) || (symbol.Kind == SymbolKind.CommentEnd));
		}

		/// <summary>
		/// Read char set information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadCharsets(LoadContext context) {
			int index = context.ReadIntegerEntry();
			charSetTable[index] = new DfaCharset(this, index, context.ReadStringEntry());
		}

		/// <summary>
		/// Read DFA state information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadDfaStates(LoadContext context) {
			DfaState dfaState = GetDfaState(context.ReadIntegerEntry());
			Symbol acceptSymbol = null;
			bool acceptState = context.ReadBoolEntry();
			if (acceptState) {
				acceptSymbol = symbolTable[context.ReadIntegerEntry()];
			} else {
				context.ReadIntegerEntry(); // Skip the entry.
			}
			context.ReadEmptyEntry();

			// Read DFA edges
			DfaEdge[] edges = new DfaEdge[context.EntryCount/3];
			for (int i = 0; i < edges.Length; i++) {
				edges[i] = new DfaEdge(context.ReadIntegerEntry(), context.ReadIntegerEntry());
				context.ReadEmptyEntry();
			}

			// Create DFA state and store it in DFA state table
			dfaState.Initialize(acceptSymbol, edges);
		}

		/// <summary>
		/// Reads grammar header information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadHeader(LoadContext context) {
			name = context.ReadStringEntry();
			version = context.ReadStringEntry();
			author = context.ReadStringEntry();
			about = context.ReadStringEntry();
			caseSensitive = context.ReadBoolEntry();
			context.StartSymbolIndex = context.ReadIntegerEntry();
		}

		/// <summary>
		/// Read initial DFA and LR states.
		/// </summary>
		/// <param name="context"></param>
		private void ReadInitialStates(LoadContext context) {
			context.DfaInitialStateIndex = context.ReadIntegerEntry();
			context.LrInitialState = context.ReadIntegerEntry();
		}

		/// <summary>
		/// Read LR state information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadLRStates(LoadContext context) {
			int index = context.ReadIntegerEntry();
			context.ReadEmptyEntry();
			LalrAction[] table = new LalrAction[context.EntryCount/4];
			for (int i = 0; i < table.Length; i++) {
				Symbol symbol = symbolTable[context.ReadIntegerEntry()];
				LalrActionType actionType = (LalrActionType)context.ReadIntegerEntry();
				int targetIndex = context.ReadIntegerEntry();
				context.ReadEmptyEntry();
				LalrAction action;
				switch (actionType) {
				case LalrActionType.Accept:
					action = new LalrActionAccept(i, symbol);
					break;
				case LalrActionType.Goto:
					action = new LalrActionGoto(i, symbol, GetLalrState(targetIndex));
					break;
				case LalrActionType.Reduce:
					action = new LalrActionReduce(i, symbol, GetRule(targetIndex));
					break;
				case LalrActionType.Shift:
					action = new LalrActionShift(i, symbol, GetLalrState(targetIndex));
					break;
				default:
					throw new InvalidOperationException("Invalid table action data");
				}
				table[i] = action;
			}
			// Create the transition vector
			LalrAction[] transitionVector = new LalrAction[symbolTable.Length];
			for (int i = 0; i < transitionVector.Length; i++) {
				transitionVector[i] = null;
			}
			for (int i = 0; i < table.Length; i++) {
				transitionVector[table[i].Symbol.Index] = table[i];
			}
			GetLalrState(index).Initialize(table, transitionVector);
		}

		/// <summary>
		/// Read char set information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadPackedCharsets(LoadContext context) {
			int index = context.ReadIntegerEntry();
			char sequenceStart = (char)context.ReadIntegerEntry();
			char sequenceEnd = (char)context.ReadIntegerEntry();
			StringBuilder result = new StringBuilder(context.ReadStringEntry());
			result.Capacity = result.Length+(sequenceEnd-sequenceStart)+1;
			for (char c = sequenceStart; c <= sequenceEnd; c++) {
				result.Append(c);
			}
			charSetTable[index] = new DfaCharset(this, index, result.ToString());
		}

		/// <summary>
		/// Read rule information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadRules(LoadContext context) {
			int index = context.ReadIntegerEntry();
			Symbol nonterminal = symbolTable[context.ReadIntegerEntry()];
			context.ReadEmptyEntry();
			Symbol[] symbols = new Symbol[context.EntryCount];
			for (int i = 0; i < symbols.Length; i++) {
				symbols[i] = symbolTable[context.ReadIntegerEntry()];
			}
			GetRule(index).Initialize(nonterminal, symbols);
		}

		/// <summary>
		/// Read symbol information.
		/// </summary>
		/// <param name="context"></param>
		private void ReadSymbols(LoadContext context) {
			int index = context.ReadIntegerEntry();
			string name = context.ReadStringEntry();
			SymbolKind symbolKind = (SymbolKind)context.ReadIntegerEntry();
			Symbol symbol = new Symbol(this, index, name, symbolKind);
			switch (symbolKind) {
			case SymbolKind.Error:
				errorSymbol = symbol;
				break;
			case SymbolKind.End:
				endSymbol = symbol;
				break;
			}
			symbolTable[index] = symbol;
		}

		/// <summary>
		/// Reads table record counts and initializes tables.
		/// </summary>
		/// <param name="context"></param>
		private void ReadTableCounts(LoadContext context) {
			// Initialize tables
			symbolTable = new Symbol[context.ReadIntegerEntry()];
			charSetTable = new DfaCharset[context.ReadIntegerEntry()];
			ruleTable = new Rule[context.ReadIntegerEntry()];
			for (int i = 0; i < ruleTable.Length; i++) {
				ruleTable[i] = new Rule(this, i);
			}
			dfaStateTable = new DfaState[context.ReadIntegerEntry()];
			for (int i = 0; i < dfaStateTable.Length; i++) {
				dfaStateTable[i] = new DfaState(this, i);
			}
			lalrStateTable = new LalrState[context.ReadIntegerEntry()];
			for (int i = 0; i < lalrStateTable.Length; i++) {
				lalrStateTable[i] = new LalrState(this, i);
			}
		}
	}
}
