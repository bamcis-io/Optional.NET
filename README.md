# BAMCIS Optional.NET

A .NET Core implementation of the Java java.util.Optional class.

## Table of Contents
- [Usage](#usage)
- [Revision History](#revision-history)

## Usage

Import the package:

    using BAMCIS.Util;

Only `class` type generics can be used with the `Optional` class (i.e. not structs) as the value needs to be nullable. 

A simple example:

    List<Optional<string>> ListOfOptionals = new List<Optional<string>>() { Optional<string>.OfNullable("TEST"), Optional<string>.OfNullable("TEST2"), Optional<string>.Empty, Optional<string>.Of("TEST3"), Optional<string>.Empty };

            
    List<string> Result = ListOfOptionals.SelectMany(x => x.Stream()).ToList();

The resulting list will have 3 elements, `TEST`, `TEST2`, and `TEST3`.

Another example:

	 string Val = "TEST";
     Optional<string> Opt = Optional<string>.OfNullable(Val);
     ...
     string Result = Opt.OrElse("FAIL");

In this case, the `Opt` variable might be supplied as an argument to a function and the function might want to provide a default value of `FAIL` when the Optional does not have a value.

## Revision History

### 1.0.0
Initial release of the library.
