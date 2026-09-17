namespace ProtocolTests.Protocol.Params.Param.Measurement.Type.CheckDateTimeAndDateParameters
{
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Data;
    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.Params.Param.Measurement.Type.CheckDateTimeAndDateParameters;

    [TestClass]
    public class Validate
    {
        private readonly IValidate test = new CheckDateTimeAndDateParameters();

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_Valid()
        {
            const string code = "<Protocol><Params><Param id='1000'><Type>array</Type><ArrayOptions><ColumnOption idx='0' pid='1001' options=';disableHeaderSum'/></ArrayOptions></Param><Param id='1001'><Type>read</Type><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display><Measurement><Type options='datetime'>number</Type></Measurement></Param><Param id='1002'><Type>read</Type><Measurement><Type options='time'>number</Type></Measurement></Param></Params></Protocol>";

            var context = new ValidatorContext(new ProtocolInputData(code), new ValidatorSettings());

            var results = test.Validate(context);

            results.Should().BeEmpty();
        }

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_Invalid()
        {
            const string code = "<Protocol><Params><Param id='1000'><Type>array</Type><ArrayOptions><ColumnOption idx='0' pid='1003' options=',foo,enableHeaderSum'/></ArrayOptions></Param><Param id='1001'><Type>read</Type><Interprete><Decimals>7</Decimals></Interprete><Display><Decimals>8</Decimals></Display><Measurement><Type options='date'>number</Type></Measurement></Param><Param id='1002'><Type>read</Type><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>9</Decimals></Display><Measurement><Type options='datetime:minute'>number</Type></Measurement></Param><Param id='1003'><Type>read</Type><Interprete><Decimals>8</Decimals></Interprete><Display><Decimals>8</Decimals></Display><Measurement><Type options='datetime'>number</Type></Measurement></Param><Param id='1004'><Type>read</Type><Measurement><Type options='time'>number</Type></Measurement></Param></Params></Protocol>";

            var context = new ValidatorContext(new ProtocolInputData(code), new ValidatorSettings());

            var results = test.Validate(context);
            var expected = new List<IValidationResult>
            {
                Error.InvalidInterpreteDecimals(test, null, null, "1001"),
                Error.InvalidDisplayDecimals(test, null, null, "1002"),
                Error.MissingDisableHeaderSum(test, null, null, "1003", "1000"),
            };

            results.Select(result => result.FullId).Should().BeEquivalentTo(expected.Select(result => result.FullId));
            results.Select(result => result.Description).Should().BeEquivalentTo(expected.Select(result => result.Description));
        }
    }

    [TestClass]
    public class Attribute
    {
        private readonly IRoot root = new CheckDateTimeAndDateParameters();

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_CheckCategory() => Generic.CheckCategory(root, Category.Param);

        [TestMethod]
        public void Param_CheckDateTimeAndDateParameters_CheckId() => Generic.CheckId(root, CheckId.CheckDateTimeAndDateParameters);
    }
}
