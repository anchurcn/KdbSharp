
// misc types testing values

alltype.boolean: (0b; 1b);
alltype.guid: (0Ng; "G"$"11223344-5566-7788-99aa-bbccddeeffaa");
alltype.byte: (0x00; 0xff);
alltype.char: (" "; "a");
alltype.symbol: (`; `a);

// numeric types testing values (zero, null, infinity, -infinity, max, min)

alltype.short: (0h; 0Nh; 0Wh; -0Wh; 32766h; -32766);
alltype.int: (0i; 0Ni; 0Wi; -0Wi; 2147483646; -2147483646);
alltype.long: (0j; 0Nj; 0Wj; -0Wj; 9223372036854775806; -9223372036854775806);
alltype.real: (0e; 0Ne; 0We; -0We; 114514.1919810e; -114514.1919810e);
alltype.float: (0f; 0Nf; 0Wf; -0Wf; 114514.1919810f; -114514.1919810f);
alltype.timestamp: "p"$alltype.long;
alltype.month: "m"$alltype.int;
alltype.date: "d"$alltype.int;
alltype.datetime: "z"$alltype.float;
alltype.timespan: "n"$alltype.long;
alltype.minute: "u"$alltype.int;
alltype.second: "v"$alltype.int;
alltype.time: "t"$alltype.int;

// Atom.
// boolean.
alltype.boolean_false: 0b;
alltype.boolean_true: 1b;
// guid.
alltype.guid_null: 0Ng;
alltype.guid_value: "G"$"11223344-5566-7788-99aa-bbccddeeffaa";
// byte.
alltype.byte_zero: 0x00;
alltype.byte_max: 0xff;
// char.
alltype.char_null: " ";
alltype.char_value: "a";
// symbol.
alltype.symbol_null: `;
alltype.symbol_value: `a;
// short.
alltype.short_zero: 0h;
alltype.short_null: 0Nh;
alltype.short_inf: 0Wh;
alltype.short_ninf: -0Wh;
alltype.short_max: 32766h;
alltype.short_min: -32766h;
// int.
alltype.int_zero: 0i;
alltype.int_null: 0Ni;
alltype.int_inf: 0Wi;
alltype.int_ninf: -0Wi;
alltype.int_max: 2147483646;
alltype.int_min: -2147483646;
// long.
alltype.long_zero: 0j;
alltype.long_null: 0Nj;
alltype.long_inf: 0Wj;
alltype.long_ninf: -0Wj;
alltype.long_max: 9223372036854775806;
alltype.long_min: -9223372036854775806;
// real.
alltype.real_zero: 0e;
alltype.real_null: 0Ne;
alltype.real_inf: 0We;
alltype.real_ninf: -0We;
alltype.real_max: 114514.1919810e;
alltype.real_min: -114514.1919810e;
// float.
alltype.float_zero: 0f;
alltype.float_null: 0Nf;
alltype.float_inf: 0Wf;
alltype.float_ninf: -0Wf;
alltype.float_max: 114514.1919810f;
alltype.float_min: -114514.1919810f;
// Temporal zero, null, infinity, -infinity, max, min.
// timestamp.
alltype.timestamp_zero: "p"$alltype.long_zero;
alltype.timestamp_null: "p"$alltype.long_null;
alltype.timestamp_inf: "p"$alltype.long_inf;
alltype.timestamp_ninf: "p"$alltype.long_ninf;
alltype.timestamp_max: "p"$alltype.long_max;
alltype.timestamp_min: "p"$alltype.long_min;
// month.
alltype.month_zero: "m"$alltype.int_zero;
alltype.month_null: "m"$alltype.int_null;
alltype.month_inf: "m"$alltype.int_inf;
alltype.month_ninf: "m"$alltype.int_ninf;
alltype.month_max: "m"$alltype.int_max;
alltype.month_min: "m"$alltype.int_min;
// date.
alltype.date_zero: "d"$alltype.int_zero;
alltype.date_null: "d"$alltype.int_null;
alltype.date_inf: "d"$alltype.int_inf;
alltype.date_ninf: "d"$alltype.int_ninf;
alltype.date_max: "d"$alltype.int_max;
alltype.date_min: "d"$alltype.int_min;
// datetime.
alltype.datetime_zero: "z"$alltype.float_zero;
alltype.datetime_null: "z"$alltype.float_null;
alltype.datetime_inf: "z"$alltype.float_inf;
alltype.datetime_ninf: "z"$alltype.float_ninf;
alltype.datetime_max: "z"$alltype.float_max;
alltype.datetime_min: "z"$alltype.float_min;
// timespan.
alltype.timespan_zero: "n"$alltype.long_zero;
alltype.timespan_null: "n"$alltype.long_null;
alltype.timespan_inf: "n"$alltype.long_inf;
alltype.timespan_ninf: "n"$alltype.long_ninf;
alltype.timespan_max: "n"$alltype.long_max;
alltype.timespan_min: "n"$alltype.long_min;
// minute.
alltype.minute_zero: "u"$alltype.int_zero;
alltype.minute_null: "u"$alltype.int_null;
alltype.minute_inf: "u"$alltype.int_inf;
alltype.minute_ninf: "u"$alltype.int_ninf;
alltype.minute_max: "u"$alltype.int_max;
alltype.minute_min: "u"$alltype.int_min;
// second.
alltype.second_zero: "v"$alltype.int_zero;
alltype.second_null: "v"$alltype.int_null;
alltype.second_inf: "v"$alltype.int_inf;
alltype.second_ninf: "v"$alltype.int_ninf;
alltype.second_max: "v"$alltype.int_max;
alltype.second_min: "v"$alltype.int_min;
// time.
alltype.time_zero: "t"$alltype.int_zero;
alltype.time_null: "t"$alltype.int_null;
alltype.time_inf: "t"$alltype.int_inf;
alltype.time_ninf: "t"$alltype.int_ninf;
alltype.time_max: "t"$alltype.int_max;
alltype.time_min: "t"$alltype.int_min;

