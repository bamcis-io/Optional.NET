using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace BAMCIS.Util.Optional.Test
{
    public class OptionalDotNetTests
    {
        private string EmptyActionString = "";

        [Fact]
        public void TestConstructor()
        {
            // ARRANGE
            string Val = "TEST";

            // ACT
            Optional<string> Opt = new Optional<string>(Val);

            // ASSERT
            Assert.True(Opt.IsPresent());
            Assert.Equal("TEST", Opt.Value);
        }

        [Fact]
        public void TestOf()
        {
            // ARRANGE
            string Val = "TEST";

            // ACT
            Optional<string> Opt = Optional<string>.Of(Val);

            // ASSERT
            Assert.True(Opt.IsPresent());
            Assert.Equal("TEST", Opt.Value);
        }

        [Fact]
        public void TestOfNullable1()
        {
            // ARRANGE
            string Val = "TEST";

            // ACT
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ASSERT
            Assert.True(Opt.IsPresent());
            Assert.Equal("TEST", Opt.Value);
        }

        [Fact]
        public void TestOfNullable2()
        {
            // ARRANGE
            string Val = null;

            // ACT
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ASSERT
            Assert.True(!Opt.IsPresent());
            Assert.Null(Opt.Value);
        }

        [Fact]
        public void TestIfPresentSuccess()
        {
            // ARRANGE
            int Val = 10;
            List<int> ValList = new List<int>() { Val };
            Optional<List<int>> Opt = Optional<List<int>>.OfNullable(ValList);

            // ACT
            Opt.IfPresent(x => x[0] += 10);

            // ASSERT
            Assert.Equal(20, Opt.Value[0]);
        }

        [Fact]
        public void TestIfPresentFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Opt.IfPresent(x => x += "ING");

            // ASSERT
            Assert.Null(Opt.Value);
        }

        [Fact]
        public void TestIfPresentOrElseSuccess()
        {
            // ARRANGE
            int Val = 10;
            List<int> ValList = new List<int>() { Val };
            Optional<List<int>> Opt = Optional<List<int>>.OfNullable(ValList);
            ThreadStart Thread = new ThreadStart(EmptyAction);

            // ACT
            Opt.IfPresentOrElse(x => x[0] += 10, Thread);

            // ASSERT
            Assert.Equal(20, Opt.Value[0]);
            Assert.Equal(String.Empty, EmptyActionString);
        }

        [Fact]
        public void TestIfPresentOrElseFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);
            ThreadStart Thread = new ThreadStart(EmptyAction);
            EmptyActionString = "";

            // ACT
            Opt.IfPresentOrElse(x => x += "ING", Thread);

            // ASSERT
            Assert.Null(Opt.Value);
            Assert.Equal("EMPTY_ACTION", EmptyActionString);
        }

        [Fact]
        public void TestFilterSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
           Optional<string> Result = Opt.Filter(x => x.StartsWith("T"));

            // ASSERT
            Assert.True(Result.IsPresent());
            Assert.Equal("TEST", Result.Value);
        }

        [Fact]
        public void TestFilterFailure()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.Filter(x => x.StartsWith("E"));

            // ASSERT
            Assert.False(Result.IsPresent());
            Assert.Null(Result.Value);
        }

        [Fact]
        public void TestMapSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.Map(x => { return x += "ING"; });

            // ASSERT
            Assert.True(Result.IsPresent());
            Assert.Equal("TESTING", Result.Value);
        }

        [Fact]
        public void TestMapFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.Map(x => { return x += "ING"; });

            // ASSERT
            Assert.False(Result.IsPresent());
            Assert.Null(Result.Value);
        }

        [Fact]
        public void TestFlatMapSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.FlatMap(x => { return new Optional<string>(x += "ING");  });

            // ASSERT
            Assert.True(Result.IsPresent());
            Assert.Equal("TESTING", Result.Value);
        }

        [Fact]
        public void TestFlatMapFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.FlatMap(x => { return new Optional<string>(x += "ING"); });

            // ASSERT
            Assert.False(Result.IsPresent());
            Assert.Null(Result.Value);
        }

        [Fact]
        public void TestStreamSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            IEnumerable<string> Result = Opt.Stream();

            // ASSERT
            Assert.True(Result.Any());
            Assert.Equal("TEST", Result.First());
        }

        [Fact]
        public void TestStreamFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            IEnumerable<string> Result = Opt.Stream();

            // ASSERT
            Assert.False(Result.Any());
        }

        [Fact]
        public void TestFilterStream()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);
            List<Optional<string>> ListOfOptionals = new List<Optional<string>>() { Optional<string>.OfNullable("TEST"), Optional<string>.OfNullable("TEST2"), Optional<string>.Empty, Optional<string>.Of("TEST3"), Optional<string>.Empty };

            // ACT
            List<string> Result = ListOfOptionals.Where(x => x.IsPresent()).Select(x => x.Value).ToList();

            // ASSERT
            Assert.Equal(3, Result.Count);
        }

        [Fact]
        public void TestFilterStream2()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);
            List<Optional<string>> ListOfOptionals = new List<Optional<string>>() { Optional<string>.OfNullable("TEST"), Optional<string>.OfNullable("TEST2"), Optional<string>.Empty, Optional<string>.Of("TEST3"), Optional<string>.Empty };

            // ACT
            List<string> Result = ListOfOptionals.SelectMany(x => x.Stream()).ToList();

            // ASSERT
            Assert.Equal(3, Result.Count);
        }

        [Fact]
        public void TestOrSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.Or(() => { return Optional<string>.Of("FAIL"); });

            // ASSERT
            Assert.True(Result.IsPresent());
            Assert.Equal("TEST", Result.Value);
        }

        [Fact]
        public void TestOrFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            Optional<string> Result = Opt.Or(() => { return Optional<string>.Of("FAIL"); });

            // ASSERT
            Assert.True(Result.IsPresent());
            Assert.Equal("FAIL", Result.Value);
        }

        [Fact]
        public void TestOrElseSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            string Result = Opt.OrElse("FAIL");

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Result));
            Assert.Equal("TEST", Result);
        }

        [Fact]
        public void TestOrElseFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            string Result = Opt.OrElse("FAIL");

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Result));
            Assert.Equal("FAIL", Result);
        }

        [Fact]
        public void TestOrElseGetSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            string Result = Opt.OrElseGet(() => { return "FAIL"; });

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Result));
            Assert.Equal("TEST", Result);
        }

        [Fact]
        public void TestOrElseGetFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            string Result = Opt.OrElseGet(() => { return "FAIL"; });

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Result));
            Assert.Equal("FAIL", Result);
        }

        [Fact]
        public void TestOrElseThrowSuccess()
        {
            // ARRANGE
            string Val = "TEST";
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT
            string Result = Opt.OrElseThrow<Exception>(() => { return new NullReferenceException("NULL VAL"); });

            // ASSERT
            Assert.True(!String.IsNullOrEmpty(Result));
            Assert.Equal("TEST", Result);
        }

        [Fact]
        public void TestOrElseThrowFailure()
        {
            // ARRANGE
            string Val = null;
            Optional<string> Opt = Optional<string>.OfNullable(Val);

            // ACT

            // ASSERT
            Assert.Throws<NullReferenceException>(() => Opt.OrElseThrow<Exception>(() => { return new NullReferenceException("NULL VAL"); }));
        }

        [Fact]
        public void TestEqualSuccess()
        {
            // ARRANGE
            string Val1 = "TEST";
            Optional<string> Opt1 = Optional<string>.OfNullable(Val1);

            string Val2 = "TEST";
            Optional<string> Opt2 = Optional<string>.OfNullable(Val2);

            // ACT

            // ASSERT
            Assert.Equal(Val1, Val2);
        }

        [Fact]
        public void TestEqualSuccess2()
        {
            // ARRANGE
            string Val1 = "TEST";
            Optional<string> Opt1 = Optional<string>.OfNullable(Val1);

            string Val2 = "TEST";
            Optional<string> Opt2 = Optional<string>.OfNullable(Val2);

            // ACT

            // ASSERT
            Assert.True(Val1 == Val2);
        }

        [Fact]
        public void TestEqualSuccess3()
        {
            // ARRANGE
            string Val1 = "TEST";
            Optional<string> Opt1 = Optional<string>.OfNullable(Val1);

            string Val2 = "TEST";
            Optional<string> Opt2 = Optional<string>.OfNullable(Val2);

            // ACT

            // ASSERT
            Assert.True(Val1.Equals(Val2));
        }

        [Fact]
        public void TestEqualFailure()
        {
            // ARRANGE
            string Val1 = "TEST";
            Optional<string> Opt1 = Optional<string>.OfNullable(Val1);

            string Val2 = "TEST1";
            Optional<string> Opt2 = Optional<string>.OfNullable(Val2);

            // ACT

            // ASSERT
            Assert.NotEqual(Val1, Val2);
        }

        [Fact]
        public void TestEqualFailure2()
        {
            // ARRANGE
            string Val1 = "TEST";
            Optional<string> Opt1 = Optional<string>.OfNullable(Val1);

            string Val2 = "TEST1";
            Optional<string> Opt2 = Optional<string>.OfNullable(Val2);

            // ACT

            // ASSERT
            Assert.False(Val1 == Val2);
        }

        [Fact]
        public void TestEqualFailure3()
        {
            // ARRANGE
            string Val1 = "TEST";
            Optional<string> Opt1 = Optional<string>.OfNullable(Val1);

            string Val2 = "TEST1";
            Optional<string> Opt2 = Optional<string>.OfNullable(Val2);

            // ACT

            // ASSERT
            Assert.False(Val1.Equals(Val2));
        }

        private void EmptyAction()
        {
            Debug.WriteLine("*****Empty Action*****");
            EmptyActionString = "EMPTY_ACTION";
        }
    }
}
