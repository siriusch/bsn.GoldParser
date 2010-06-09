// (C) 2010 Arsène von Wyss / bsn
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace bsn.GoldParser.Grammar {
	/// <summary>
	/// Contains grammar tables required for parsing.
	/// </summary>
	/// <remarks>The code is inspired by the GOLD C# engine by Vladimir Morozov (vmoroz@hotmail.com)</remarks>
	public class CompiledGrammar {
		/// <summary>
		/// Entry type byte in the binary grammar file.
		/// </summary>
		private enum EntryType {
			Empty = (int)'E', // 69
			Integer = (int)'I', // 73
			String = (int)'S', // 83
			Boolean = (int)'B', // 66
			Byte = (int)'b' // 98
		}

		/// <summary>
		/// Record type byte in the binary grammar file.
		/// </summary>
		private enum RecordType {
			Parameters = (int)'P', // 80
			TableCounts = (int)'T', // 84
			Initial = (int)'I', // 73
			Symbols = (int)'S', // 83
			CharSets = (int)'C', // 67
			Rules = (int)'R', // 82
			DfaStates = (int)'D', // 68
			LRStates = (int)'L', // 76
			Comment = (int)'!' // 33
		}

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

		/// <summary>
		/// Loads the CompiledGrammar from the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public static CompiledGrammar Load(Stream stream) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
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

		private readonly BinaryReader reader; // Source of the grammar    
		private readonly object sync = new object();

		// Grammar header information
		private string about; // Grammar description
		private string author; // Author of the grammar
		private bool caseSensitive; // Grammar is case sensitive or not

		// Tables read from the binary grammar file
		private String[] charSetTable; // Charset table
		private DfaState dfaInitialState; // DFA initial state 
		private int dfaInitialStateIndex; // DFA initial state index
		private DfaState[] dfaStateTable; // DFA state table
		private Symbol endSymbol;
		private int entryCount; // Number of entries left
		private Symbol errorSymbol;
		private LalrState[] lalrStateTable; // LR state table
		private int lrInitialState; // LR initial state
		private string name; // Name of the grammar
		private Rule[] ruleTable; // Rule table
		private Dictionary<Symbol, ReadOnlyCollection<Rule>> rules;
		private int startSymbolIndex; // Start symbol index
		private Symbol[] symbolTable; // Symbol table

		// helper data structures for optimized access/query
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

			this.reader = reader;
			Load();
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

		/// <summary>
		/// Gets the value indicating if the grammar is case sensitive.
		/// </summary>
		public bool CaseSensitive {
			get {
				return caseSensitive;
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
				return lalrStateTable[lrInitialState];
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
		/// Gets the start symbol for the grammar.
		/// </summary>
		public Symbol StartSymbol {
			get {
				return symbolTable[startSymbolIndex];
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
		/// Gets the dfa charset count.
		/// </summary>
		/// <value>The dfa charset count.</value>
		protected internal int DfaCharsetCount {
			get {
				return charSetTable.Length;
			}
		}

		/// <summary>
		/// Gets the dfa state count.
		/// </summary>
		/// <value>The dfa state count.</value>
		protected internal int DfaStateCount {
			get {
				return dfaStateTable.Length;
			}
		}

		/// <summary>
		/// Gets the lalr state count.
		/// </summary>
		/// <value>The lalr state count.</value>
		protected internal int LalrStateCount {
			get {
				return lalrStateTable.Length;
			}
		}

		/// <summary>
		/// Gets the rule count.
		/// </summary>
		/// <value>The rule count.</value>
		protected internal int RuleCount {
			get {
				return ruleTable.Length;
			}
		}

		/// <summary>
		/// Gets the symbol count.
		/// </summary>
		/// <value>The symbol count.</value>
		protected internal int SymbolCount {
			get {
				return symbolTable.Length;
			}
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
		/// Gets the dfa charset.
		/// </summary>
		/// <param name="charSetIndex">Index of the char set.</param>
		/// <returns></returns>
		protected internal string GetDfaCharset(int charSetIndex) {
			return charSetTable[charSetIndex];
		}

		/// <summary>
		/// Gets the state of the dfa.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		protected internal DfaState GetDfaState(int index) {
			return dfaStateTable[index];
		}

		/// <summary>
		/// Gets the state of the lalr.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		protected internal LalrState GetLalrState(int index) {
			return lalrStateTable[index];
		}

		/// <summary>
		/// Gets the rule.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		protected internal Rule GetRule(int index) {
			return ruleTable[index];
		}

		/// <summary>
		/// Gets the symbol.
		/// </summary>
		/// <param name="symbolIndex">Index of the symbol.</param>
		/// <returns></returns>
		protected internal Symbol GetSymbol(int symbolIndex) {
			return symbolTable[symbolIndex];
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
		private void Load() {
			if (FileHeader != ReadString()) {
				throw new FileLoadException("The File Header is invalid");
			}
			while (reader.PeekChar() != -1) {
				RecordType recordType = ReadNextRecord();
				switch (recordType) {
				case RecordType.Parameters:
					ReadHeader();
					break;

				case RecordType.TableCounts:
					ReadTableCounts();
					break;

				case RecordType.Initial:
					ReadInitialStates();
					break;

				case RecordType.Symbols:
					ReadSymbols();
					break;

				case RecordType.CharSets:
					ReadCharSets();
					break;

				case RecordType.Rules:
					ReadRules();
					break;

				case RecordType.DfaStates:
					ReadDfaStates();
					break;

				case RecordType.LRStates:
					ReadLRStates();
					break;

				default:
					throw new FileLoadException("Invalid record count");
				}
			}
			dfaInitialState = dfaStateTable[dfaInitialStateIndex];
		}

		/// <summary>
		/// Reads boolean from the grammar file.
		/// </summary>
		/// <returns>Boolean value.</returns>
		private bool ReadBool() {
			return (ReadByte() == 1);
		}

		/// <summary>
		/// Reads boolean entry from the grammar file.
		/// </summary>
		/// <returns>Boolean entry content.</returns>
		private bool ReadBoolEntry() {
			if (ReadEntryType() != EntryType.Boolean) {
				throw new FileLoadException("Boolean entry expected");
			}
			entryCount--;
			return ReadBool();
		}

		/// <summary>
		/// Reads byte from the grammar file.
		/// </summary>
		/// <returns>Byte value.</returns>
		private byte ReadByte() {
			return reader.ReadByte();
		}

		/// <summary>
		/// Reads byte entry from the grammar file.
		/// </summary>
		/// <returns>Byte entry content.</returns>
		private byte ReadByteEntry() {
			if (ReadEntryType() != EntryType.Byte) {
				throw new FileLoadException("Byte entry expected");
			}
			entryCount--;
			return ReadByte();
		}

		/// <summary>
		/// Read char set information.
		/// </summary>
		private void ReadCharSets() {
			charSetTable[ReadInt16Entry()] = ReadStringEntry();
		}

		/// <summary>
		/// Read DFA state information.
		/// </summary>
		private void ReadDfaStates() {
			DfaState dfaState = GetDfaState(ReadInt16Entry());
			Symbol acceptSymbol = null;
			bool acceptState = ReadBoolEntry();
			if (acceptState) {
				acceptSymbol = symbolTable[ReadInt16Entry()];
			} else {
				ReadInt16Entry(); // Skip the entry.
			}
			ReadEmptyEntry();

			// Read DFA edges
			DfaEdge[] edges = new DfaEdge[entryCount/3];
			for (int i = 0; i < edges.Length; i++) {
				edges[i] = new DfaEdge(ReadInt16Entry(), ReadInt16Entry());
				ReadEmptyEntry();
			}

			// Create DFA state and store it in DFA state table
			dfaState.Initialize(this, acceptSymbol, edges);
		}

		/// <summary>
		/// Reads empty entry from the grammar file.
		/// </summary>
		private void ReadEmptyEntry() {
			if (ReadEntryType() != EntryType.Empty) {
				throw new FileLoadException("Empty entry expected");
			}
			entryCount--;
		}

		/// <summary>
		/// Reads entry type.
		/// </summary>
		/// <returns>Entry type.</returns>
		private EntryType ReadEntryType() {
			if (entryCount == 0) {
				throw new FileLoadException("No entry found");
			}
			return (EntryType)ReadByte();
		}

		/// <summary>
		/// Reads grammar header information.
		/// </summary>
		private void ReadHeader() {
			name = ReadStringEntry();
			version = ReadStringEntry();
			author = ReadStringEntry();
			about = ReadStringEntry();
			caseSensitive = ReadBoolEntry();
			startSymbolIndex = ReadInt16Entry();
		}

		/// <summary>
		/// Read initial DFA and LR states.
		/// </summary>
		private void ReadInitialStates() {
			dfaInitialStateIndex = ReadInt16Entry();
			lrInitialState = ReadInt16Entry();
		}

		/// <summary>
		/// Reads two byte integer Int16 from the grammar file.
		/// </summary>
		/// <returns>Int16 value.</returns>
		private int ReadInt16() {
			return reader.ReadUInt16();
		}

		/// <summary>
		/// Reads Int16 entry from the grammar file.
		/// </summary>
		/// <returns>Int16 entry content.</returns>
		private int ReadInt16Entry() {
			if (ReadEntryType() != EntryType.Integer) {
				throw new FileLoadException("Integer entry expected");
			}
			entryCount--;
			return ReadInt16();
		}

		/// <summary>
		/// Read LR state information.
		/// </summary>
		private void ReadLRStates() {
			int index = ReadInt16Entry();
			ReadEmptyEntry();
			LalrAction[] table = new LalrAction[entryCount/4];
			for (int i = 0; i < table.Length; i++) {
				Symbol symbol = symbolTable[ReadInt16Entry()];
				LalrActionType actionType = (LalrActionType)ReadInt16Entry();
				int targetIndex = ReadInt16Entry();
				ReadEmptyEntry();
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
		/// Reads the next record in the binary grammar file.
		/// </summary>
		/// <returns>Read record type.</returns>
		private RecordType ReadNextRecord() {
			char recordType = (char)ReadByte();
			//Structure below is ready for future expansion
			switch (recordType) {
			case 'M':
				//Read the number of entry's
				entryCount = ReadInt16();
				return (RecordType)ReadByteEntry();

			default:
				throw new FileLoadException("Invalid record header");
			}
		}

		/// <summary>
		/// Read rule information.
		/// </summary>
		private void ReadRules() {
			int index = ReadInt16Entry();
			Symbol nonterminal = symbolTable[ReadInt16Entry()];
			ReadEmptyEntry();
			Symbol[] symbols = new Symbol[entryCount];
			for (int i = 0; i < symbols.Length; i++) {
				symbols[i] = symbolTable[ReadInt16Entry()];
			}
			GetRule(index).Initialize(nonterminal, symbols);
		}

		/// <summary>
		/// Reads string from the grammar file.
		/// </summary>
		/// <returns>String value.</returns>
		private string ReadString() {
			StringBuilder result = new StringBuilder();
			char unicodeChar = (char)ReadInt16();
			while (unicodeChar != (char)0) {
				result.Append(unicodeChar);
				unicodeChar = (char)ReadInt16();
			}
			return result.ToString();
		}

		/// <summary>
		/// Reads string entry from the grammar file.
		/// </summary>
		/// <returns>String entry content.</returns>
		private string ReadStringEntry() {
			if (ReadEntryType() != EntryType.String) {
				throw new FileLoadException("String entry expected");
			}
			entryCount--;
			return ReadString();
		}

		/// <summary>
		/// Read symbol information.
		/// </summary>
		private void ReadSymbols() {
			int index = ReadInt16Entry();
			string name = ReadStringEntry();
			SymbolKind symbolKind = (SymbolKind)ReadInt16Entry();
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
		private void ReadTableCounts() {
			// Initialize tables
			symbolTable = new Symbol[ReadInt16Entry()];
			charSetTable = new String[ReadInt16Entry()];
			ruleTable = new Rule[ReadInt16Entry()];
			for (int i = 0; i < ruleTable.Length; i++) {
				ruleTable[i] = new Rule(this, i);
			}
			dfaStateTable = new DfaState[ReadInt16Entry()];
			for (int i = 0; i < dfaStateTable.Length; i++) {
				dfaStateTable[i] = new DfaState(this, i);
			}
			lalrStateTable = new LalrState[ReadInt16Entry()];
			for (int i = 0; i < lalrStateTable.Length; i++) {
				lalrStateTable[i] = new LalrState(this, i);
			}
		}

		public override string ToString() {
			return Name;
		}
	}
}