//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:\szko�a\4rok\MIW\AGDSPresentationDB\Parser\Query.g4 by ANTLR 4.3

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

namespace AGDSPresentationDB {
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.3")]
[System.CLSCompliant(false)]
public partial class QueryLexer : Lexer {
	public const int
		LOGIC=1, OPERATOR=2, EQUALS=3, LEFTBRACKET=4, RIGHTBRACKET=5, NUMBER=6, 
		QUOTEDSTRING=7, STRING=8, WS=9;
	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] tokenNames = {
		"'\\u0000'", "'\\u0001'", "'\\u0002'", "'\\u0003'", "'\\u0004'", "'\\u0005'", 
		"'\\u0006'", "'\\u0007'", "'\b'", "'\t'"
	};
	public static readonly string[] ruleNames = {
		"LOGIC", "OPERATOR", "EQUALS", "LEFTBRACKET", "RIGHTBRACKET", "NUMBER", 
		"QUOTEDSTRING", "STRING", "WS"
	};


	public QueryLexer(ICharStream input)
		: base(input)
	{
		_interp = new LexerATNSimulator(this,_ATN);
	}

	public override string GrammarFileName { get { return "Query.g4"; } }

	public override string[] TokenNames { get { return tokenNames; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override string SerializedAtn { get { return _serializedATN; } }

	public static readonly string _serializedATN =
		"\x3\xAF6F\x8320\x479D\xB75C\x4880\x1605\x191C\xAB37\x2\vi\b\x1\x4\x2\t"+
		"\x2\x4\x3\t\x3\x4\x4\t\x4\x4\x5\t\x5\x4\x6\t\x6\x4\a\t\a\x4\b\t\b\x4\t"+
		"\t\t\x4\n\t\n\x3\x2\x3\x2\x3\x2\x3\x2\x3\x2\x5\x2\x1B\n\x2\x3\x3\x3\x3"+
		"\x3\x3\x3\x3\x3\x3\x3\x3\x5\x3#\n\x3\x3\x4\x3\x4\x3\x5\x3\x5\x3\x6\x3"+
		"\x6\x3\a\x3\a\x3\a\a\a.\n\a\f\a\xE\a\x31\v\a\x5\a\x33\n\a\x3\a\x3\a\x6"+
		"\a\x37\n\a\r\a\xE\a\x38\x3\a\x3\a\a\a=\n\a\f\a\xE\a@\v\a\x3\a\x3\a\a\a"+
		"\x44\n\a\f\a\xE\aG\v\a\x5\aI\n\a\x5\aK\n\a\x3\b\x3\b\x3\b\x3\b\x6\bQ\n"+
		"\b\r\b\xE\bR\x3\b\x3\b\x3\t\x6\tX\n\t\r\t\xE\tY\x3\t\x6\t]\n\t\r\t\xE"+
		"\t^\x5\t\x61\n\t\x3\n\x6\n\x64\n\n\r\n\xE\n\x65\x3\n\x3\n\x2\x2\x2\v\x3"+
		"\x2\x3\x5\x2\x4\a\x2\x5\t\x2\x6\v\x2\a\r\x2\b\xF\x2\t\x11\x2\n\x13\x2"+
		"\v\x3\x2\v\x4\x2>>@@\x3\x2\x32\x32\x4\x2\x30\x30^^\x3\x2\x32;\x3\x2\x33"+
		";\x3\x2))\x5\x2\x30\x30\x43\\\x63|\x4\x2\x43\\\x63|\x4\x2\v\v\"\"z\x2"+
		"\x3\x3\x2\x2\x2\x2\x5\x3\x2\x2\x2\x2\a\x3\x2\x2\x2\x2\t\x3\x2\x2\x2\x2"+
		"\v\x3\x2\x2\x2\x2\r\x3\x2\x2\x2\x2\xF\x3\x2\x2\x2\x2\x11\x3\x2\x2\x2\x2"+
		"\x13\x3\x2\x2\x2\x3\x1A\x3\x2\x2\x2\x5\"\x3\x2\x2\x2\a$\x3\x2\x2\x2\t"+
		"&\x3\x2\x2\x2\v(\x3\x2\x2\x2\rJ\x3\x2\x2\x2\xFL\x3\x2\x2\x2\x11`\x3\x2"+
		"\x2\x2\x13\x63\x3\x2\x2\x2\x15\x16\a\x63\x2\x2\x16\x17\ap\x2\x2\x17\x1B"+
		"\a\x66\x2\x2\x18\x19\aq\x2\x2\x19\x1B\at\x2\x2\x1A\x15\x3\x2\x2\x2\x1A"+
		"\x18\x3\x2\x2\x2\x1B\x4\x3\x2\x2\x2\x1C#\a?\x2\x2\x1D\x1E\a@\x2\x2\x1E"+
		"#\a?\x2\x2\x1F \a>\x2\x2 #\a?\x2\x2!#\t\x2\x2\x2\"\x1C\x3\x2\x2\x2\"\x1D"+
		"\x3\x2\x2\x2\"\x1F\x3\x2\x2\x2\"!\x3\x2\x2\x2#\x6\x3\x2\x2\x2$%\a?\x2"+
		"\x2%\b\x3\x2\x2\x2&\'\a*\x2\x2\'\n\x3\x2\x2\x2()\a+\x2\x2)\f\x3\x2\x2"+
		"\x2*\x32\t\x3\x2\x2+/\t\x4\x2\x2,.\t\x5\x2\x2-,\x3\x2\x2\x2.\x31\x3\x2"+
		"\x2\x2/-\x3\x2\x2\x2/\x30\x3\x2\x2\x2\x30\x33\x3\x2\x2\x2\x31/\x3\x2\x2"+
		"\x2\x32+\x3\x2\x2\x2\x32\x33\x3\x2\x2\x2\x33K\x3\x2\x2\x2\x34\x36\t\x4"+
		"\x2\x2\x35\x37\t\x5\x2\x2\x36\x35\x3\x2\x2\x2\x37\x38\x3\x2\x2\x2\x38"+
		"\x36\x3\x2\x2\x2\x38\x39\x3\x2\x2\x2\x39K\x3\x2\x2\x2:>\t\x6\x2\x2;=\t"+
		"\x5\x2\x2<;\x3\x2\x2\x2=@\x3\x2\x2\x2><\x3\x2\x2\x2>?\x3\x2\x2\x2?H\x3"+
		"\x2\x2\x2@>\x3\x2\x2\x2\x41\x45\t\x4\x2\x2\x42\x44\t\x5\x2\x2\x43\x42"+
		"\x3\x2\x2\x2\x44G\x3\x2\x2\x2\x45\x43\x3\x2\x2\x2\x45\x46\x3\x2\x2\x2"+
		"\x46I\x3\x2\x2\x2G\x45\x3\x2\x2\x2H\x41\x3\x2\x2\x2HI\x3\x2\x2\x2IK\x3"+
		"\x2\x2\x2J*\x3\x2\x2\x2J\x34\x3\x2\x2\x2J:\x3\x2\x2\x2K\xE\x3\x2\x2\x2"+
		"LP\a)\x2\x2MQ\n\a\x2\x2NO\a^\x2\x2OQ\a)\x2\x2PM\x3\x2\x2\x2PN\x3\x2\x2"+
		"\x2QR\x3\x2\x2\x2RP\x3\x2\x2\x2RS\x3\x2\x2\x2ST\x3\x2\x2\x2TU\a)\x2\x2"+
		"U\x10\x3\x2\x2\x2VX\t\b\x2\x2WV\x3\x2\x2\x2XY\x3\x2\x2\x2YW\x3\x2\x2\x2"+
		"YZ\x3\x2\x2\x2Z\x61\x3\x2\x2\x2[]\t\t\x2\x2\\[\x3\x2\x2\x2]^\x3\x2\x2"+
		"\x2^\\\x3\x2\x2\x2^_\x3\x2\x2\x2_\x61\x3\x2\x2\x2`W\x3\x2\x2\x2`\\\x3"+
		"\x2\x2\x2\x61\x12\x3\x2\x2\x2\x62\x64\t\n\x2\x2\x63\x62\x3\x2\x2\x2\x64"+
		"\x65\x3\x2\x2\x2\x65\x63\x3\x2\x2\x2\x65\x66\x3\x2\x2\x2\x66g\x3\x2\x2"+
		"\x2gh\b\n\x2\x2h\x14\x3\x2\x2\x2\x12\x2\x1A\"/\x32\x38>\x45HJPRY^`\x65"+
		"\x3\b\x2\x2";
	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN.ToCharArray());
}
} // namespace AGDSPresentationDB
