namespace ProtocolTests.Protocol.CheckEndlessLoop
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Validators.Common.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Common.Model;
    using Skyline.DataMiner.CICD.Validators.Protocol.Common;
    using Skyline.DataMiner.CICD.Validators.Protocol.Interfaces;
    using Skyline.DataMiner.CICD.Validators.Protocol.Tests.Protocol.CheckEndlessLoop;

    [TestClass]
    public class Validate
    {
        private readonly IValidate check = new CheckEndlessLoop();

        #region Valid Checks

        [TestMethod]
        public void Protocol_CheckEndlessLoop_Valid()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Trigger_CheckEndlessLoop_ValidActionAggregate()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidActionAggregate",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Trigger_CheckEndlessLoop_ValidActionMerge()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "ValidActionMerge",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckEndlessLoop_Valid_Collab269212()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Valid,
                FileName = "Valid_Collab269212",
                ExpectedResults = new List<IValidationResult>()
            };

            Generic.Validate(check, data);
        }

        #endregion

        #region Invalid Checks

        [TestMethod]
        public void Protocol_CheckEndlessLoop_EndlessLoop()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EndlessLoop",
                ExpectedResults = new List<IValidationResult>
                {
                    // Case 1:
                    Error.EndlessLoop(null,null,null, @"Trigger 100->Action 100->Param 100->Trigger 100"),
                    Error.EndlessLoop(null,null,null, @"Trigger 101->Action 101->Param 101->Trigger 101"),
                    Error.EndlessLoop(null,null,null, @"Trigger 102->Action 102->Param 102->Trigger 102"),
                    Error.EndlessLoop(null,null,null, @"Trigger 103->Action 103->Param 103->Trigger 103"),
                    //Error.EndlessLoop(null,null,null, @"Trigger 104->Action 104->Param 104->Trigger 104"),        // Comment out as copy actions don't cause trigger to go off.
                    Error.EndlessLoop(null,null,null, @"Trigger 105->Action 105->Param 105->Trigger 105"),

                    Error.EndlessLoop(null,null,null, @"Trigger 700->Action 700->Param 701->Trigger 701->Action 701->Param 700->Trigger 700"),

                    // Below one is considered duplicate of the above one so we decided not to return it.
                    //Error.EndlessLoop(null,null,null, @"Trigger 701->Action 701->Param 700->Trigger 700->Action 700->Param 701->Trigger 701"),
                    
                    Error.EndlessLoop(null,null,null, @"Action 801->Param 801->Trigger 801->Action 801"),
                    // Below one is considered an unfiltered duplicate of the above one. 
                    //Error.EndlessLoop(null,null,null, @"Trigger 800->Action 801->Param 801->Trigger 801->Action 801"),
                    
                    // Below one is considered duplicate of the above one so we decided not to return it.
                    //Error.EndlessLoop(null,null,null, @"Trigger 801->Action 801->Param 801->Trigger 801"),

                    // Case 2:
                    Error.EndlessLoop(null,null,null, @"Trigger 1000->Trigger 1001->Action 1000->Param 1000->Trigger 1000"),
                    
                    // Below one is considered duplicate of the above one so we decided not to return it.
                    //Error.EndlessLoop(null,null,null, @"Trigger 1001->Action 1000->Param 1000->Trigger 1000->Trigger 1001"),

                    // Case 3:
                    Error.EndlessLoop(null,null,null, @"Trigger 2000->Action 2000->Group 2000->Action 2001->Param 2000->Trigger 2000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 2002->Action 2002->Group 2002->Action 2003->Param 2002->Trigger 2002"),
                    
                    // Case 4:
                    Error.EndlessLoop(null,null,null, @"Trigger 3000->Action 3000->Group 3000->Param 3000->Trigger 3000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 3001->Action 3001->Group 3001->Param 3001->Trigger 3001"),
                    Error.EndlessLoop(null,null,null, @"Trigger 3002->Action 3002->Group 3002->Param 3002->Trigger 3002"),
                    Error.EndlessLoop(null,null,null, @"Trigger 3003->Action 3003->Group 3003->Param 3003->Trigger 3003"),

                    Error.EndlessLoop(null,null,null, @"Trigger 3004->Action 3004->Group 3004->Param 3004->Trigger 3004"),
                    Error.EndlessLoop(null,null,null, @"Action 3004->Group 3004->Param 3005->Trigger 3005->Action 3004"),
                    // Below one is considered an unfiltered duplicate of the above one. 
                    //Error.EndlessLoop(null,null,null, @"Trigger 3004->Action 3004->Group 3004->Param 3005->Trigger 3005->Action 3004"),
                    
                    // Below ones are considered duplicate of the above ones so we decided not to return it.
                    //Error.EndlessLoop(null,null,null, @"Trigger 3005->Action 3004->Group 3004->Param 3004->Trigger 3004->Action 3004"),
                    //Error.EndlessLoop(null,null,null, @"Trigger 3005->Action 3004->Group 3004->Param 3005->Trigger 3005"),

                    // Case 5:
                    Error.EndlessLoop(null,null,null, @"Trigger 4000->Action 4000->Group 4000->Trigger 4000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 4001->Action 4001->Group 4001->Trigger 4001"),
                    Error.EndlessLoop(null,null,null, @"Trigger 4002->Action 4002->Group 4002->Trigger 4002"),
                    Error.EndlessLoop(null,null,null, @"Trigger 4003->Action 4003->Group 4003->Trigger 4003"),

                    // Case 6:
                    Error.EndlessLoop(null,null,null, @"Trigger 5000->Action 5000->Group 5000->Pair 5000->Response 5000->Param 5000->Trigger 5000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5001->Action 5001->Group 5001->Pair 5001->Response 5001->Param 5001->Trigger 5001"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5002->Action 5002->Group 5002->Pair 5002->Response 5002->Param 5002->Trigger 5002"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5003->Action 5003->Group 5003->Pair 5003->Response 5003->Param 5003->Trigger 5003"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5004->Action 5004->Group 5004->Pair 5004->Response 5004->Param 5004->Trigger 5004"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5005->Action 5005->Group 5005->Pair 5005->Response 5005->Param 5005->Trigger 5005"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5006->Action 5006->Group 5006->Pair 5006->Response 5006->Param 5006->Trigger 5006"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5007->Action 5007->Group 5007->Pair 5007->Response 5007->Param 5007->Trigger 5007"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5008->Action 5008->Group 5008->Pair 5008->Response 5008->Param 5008->Trigger 5008"),
                    Error.EndlessLoop(null,null,null, @"Trigger 5009->Action 5009->Group 5009->Pair 5009->Response 5009->Param 5009->Trigger 5009"),

                    // Case 7:
                    Error.EndlessLoop(null,null,null, @"Trigger 6000->Action 6000->Group 6000->Pair 6000->Response 6000->Trigger 6000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 6001->Action 6001->Group 6001->Pair 6001->Response 6001->Trigger 6001"),
                    Error.EndlessLoop(null,null,null, @"Trigger 6002->Action 6002->Group 6002->Pair 6002->Response 6002->Trigger 6002"),

                    // Case 8:
                    Error.EndlessLoop(null,null,null, @"Trigger 7000->Action 7000->Group 7000->Pair 7000->Command 7000->Trigger 7000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 7000->Action 7000->Group 7000->Pair 7000->Response 7000->Trigger 7000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 7001->Action 7001->Group 7001->Pair 7001->Trigger 7001"),
                    Error.EndlessLoop(null,null,null, @"Trigger 7002->Action 7002->Group 7002->Pair 7002->Trigger 7002"),

                    // Case 9:
                    Error.EndlessLoop(null,null,null, @"Trigger 8000->Action 8000->Group 8000->Session 8000->Param 8000->Trigger 8000"),
                    Error.EndlessLoop(null,null,null, @"Trigger 8001->Action 8001->Group 8001->Session 8001->Param 8001->Trigger 8001"),

                    // Case 10:
                    Error.EndlessLoop(null,null,null, @"Trigger 9000->Action 9000->Group 9000->Session 9000->Trigger 9000"),
                   
                    // Case 100:
                    Error.EndlessLoop(null,null,null, @"Action 1000000->Group 1000000->Action 1000000"),
                    // Below one is considered an unfiltered duplicate of the above one. 
                    //Error.EndlessLoop(null,null,null, @"Trigger 1000000->Action 1000000->Group 1000000->Action 1000000"),

                    // Case 101:
                    Error.EndlessLoop(null,null,null, @"Action 1010000->Group 1010000->Action 1010000"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckEndlessLoop_EndlessLoopActionAggregate()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EndlessLoopActionAggregate",
                ExpectedResults = new List<IValidationResult>
                {
                    Error.EndlessLoop(null,null,null, @"Action 210->Param 250->Trigger 310->Action 210"),
                    Error.EndlessLoop(null,null,null, @"Trigger 201->Action 201->Param 201->Trigger 201"),
                    Error.EndlessLoop(null,null,null, @"Trigger 202->Action 202->Param 242->Trigger 202"),
                    Error.EndlessLoop(null,null,null, @"Trigger 203->Action 203->Param 203->Trigger 203"),
                    Error.EndlessLoop(null,null,null, @"Trigger 204->Action 204->Param 204->Trigger 204"),
                    Error.EndlessLoop(null,null,null, @"Trigger 205->Action 205->Param 245->Trigger 205"),

                    Error.EndlessLoop(null,null,null, @"Trigger 210->Action 210->Param 210->Trigger 210"),
                    Error.EndlessLoop(null,null,null, @"Trigger 211->Action 211->Param 251->Trigger 211"),
                    Error.EndlessLoop(null,null,null, @"Trigger 212->Action 212->Param 252->Trigger 212"),
                    Error.EndlessLoop(null,null,null, @"Trigger 213->Action 213->Param 253->Trigger 213"),
                    Error.EndlessLoop(null,null,null, @"Trigger 214->Action 214->Param 254->Trigger 214"),
                    Error.EndlessLoop(null,null,null, @"Trigger 215->Action 215->Param 255->Trigger 215"),
                    Error.EndlessLoop(null,null,null, @"Trigger 216->Action 216->Param 276->Trigger 216"),
                    Error.EndlessLoop(null,null,null, @"Trigger 217->Action 217->Param 257->Trigger 217"),
                    Error.EndlessLoop(null,null,null, @"Trigger 218->Action 218->Param 258->Trigger 218"),
                    Error.EndlessLoop(null,null,null, @"Trigger 219->Action 219->Param 309->Trigger 219"),
                    Error.EndlessLoop(null,null,null, @"Trigger 221->Action 221->Param 301->Trigger 221"),
                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        [Ignore("toBeImplemented")]
        public void Protocol_CheckEndlessLoop_EndlessLoopActionMerge()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "EndlessLoopActionMerge",
                ExpectedResults = new List<IValidationResult>
                {

                }
            };

            Generic.Validate(check, data);
        }

        [TestMethod]
        public void Protocol_CheckEndlessLoop_PotentialEndlessLoop()
        {
            Generic.ValidateData data = new Generic.ValidateData
            {
                TestType = Generic.TestType.Invalid,
                FileName = "PotentialEndlessLoop",
                ExpectedResults = new List<IValidationResult>
                {
                    // Case 1:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 100->Action 100->Param 100->Trigger 100"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 101->Action 101->Param 101->Trigger 101"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 102->Action 102->Param 102->Trigger 102"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 103->Action 103->Param 103->Trigger 103"),
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 104->Action 104->Param 104->Trigger 104"),        // Comment out as copy actions don't cause trigger to go off.
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 105->Action 105->Param 105->Trigger 105"),

                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 700->Action 700->Param 701->Trigger 701->Action 701->Param 700->Trigger 700"),

                    // Below one is considered duplicate of the above one so we decided not to return it.
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 151->Action 151->Param 150->Trigger 150->Action 150->Param 151->Trigger 151"),

                     Error.PotentialEndlessLoop(null,null,null, @"Action 801->Param 801->Trigger 801->Action 801"),
                    // Below one is considered an unfiltered version of the above one.
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 200->Action 201->Param 201->Trigger 201->Action 201"),

                    // Below one is considered duplicate of the above one so we decided not to return it.
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 201->Action 201->Param 201->Trigger 201"),

                    // Case 2:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 1000->Trigger 1001->Action 1000->Param 1000->Trigger 1000"),

                    // Below one is considered duplicate of the above one so we decided not to return it.
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 1001->Action 1000->Param 1000->Trigger 1000->Trigger 1001"),

                    // Case 3:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 2000->Action 2000->Group 2000->Action 2001->Param 2000->Trigger 2000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 2002->Action 2002->Group 2002->Action 2003->Param 2002->Trigger 2002"),
                    
                    // Case 4:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 3000->Action 3000->Group 3000->Param 3000->Trigger 3000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 3001->Action 3001->Group 3001->Param 3001->Trigger 3001"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 3002->Action 3002->Group 3002->Param 3002->Trigger 3002"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 3003->Action 3003->Group 3003->Param 3003->Trigger 3003"),

                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 3004->Action 3004->Group 3004->Param 3004->Trigger 3004"),
                    Error.PotentialEndlessLoop(null,null,null, @"Action 3004->Group 3004->Param 3005->Trigger 3005->Action 3004"),
                    // Below one is considered an unfiltered version of the above one.
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 3004->Action 3004->Group 3004->Param 3005->Trigger 3005->Action 3004"),
                    
                    // Below ones are considered duplicate of the above ones so we decided not to return it.
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 3005->Action 3004->Group 3004->Param 3004->Trigger 3004->Action 3004"),
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 3005->Action 3004->Group 3004->Param 3005->Trigger 3005"),

                    // Case 5:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 4000->Action 4000->Group 4000->Trigger 4000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 4001->Action 4001->Group 4001->Trigger 4001"),

                    // Case 6:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5000->Action 5000->Group 5000->Pair 5000->Response 5000->Param 5000->Trigger 5000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5001->Action 5001->Group 5001->Pair 5001->Response 5001->Param 5001->Trigger 5001"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5002->Action 5002->Group 5002->Pair 5002->Response 5002->Param 5002->Trigger 5002"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5003->Action 5003->Group 5003->Pair 5003->Response 5003->Param 5003->Trigger 5003"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5004->Action 5004->Group 5004->Pair 5004->Response 5004->Param 5004->Trigger 5004"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5005->Action 5005->Group 5005->Pair 5005->Response 5005->Param 5005->Trigger 5005"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 5006->Action 5006->Group 5006->Pair 5006->Response 5006->Param 5006->Trigger 5006"),

                    // Case 7:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 6000->Action 6000->Group 6000->Pair 6000->Response 6000->Trigger 6000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 6001->Action 6001->Group 6001->Pair 6001->Response 6001->Trigger 6001"),

                    // Case 8:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 7000->Action 7000->Group 7000->Pair 7000->Command 7000->Trigger 7000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 7000->Action 7000->Group 7000->Pair 7000->Response 7000->Trigger 7000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 7001->Action 7001->Group 7001->Pair 7001->Trigger 7001"),

                    // Case 9:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 8000->Action 8000->Group 8000->Session 8000->Param 8000->Trigger 8000"),
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 8001->Action 8001->Group 8001->Session 8001->Param 8001->Trigger 8001"),

                    // Case 10:
                    Error.PotentialEndlessLoop(null,null,null, @"Trigger 9000->Action 9000->Group 9000->Session 9000->Trigger 9000"),

                    // Case 100:
                    Error.PotentialEndlessLoop(null,null,null, @"Action 1000000->Group 1000000->Action 1000000"),
                    // Below one is considered an unfiltered version of the above one.                    
                    //Error.PotentialEndlessLoop(null,null,null, @"Trigger 1000000->Action 1000000->Group 1000000->Action 1000000"),

                    // Case 101:
                    Error.PotentialEndlessLoop(null,null,null, @"Action 1010000->Group 1010000->Action 1010000"),
                }
            };

            Generic.Validate(check, data);
        }

        #endregion
    }

    [TestClass]
    public class ErrorMessages
    {
        [TestMethod]

        public void Protocol_CheckEndlessLoop_EndlessLoop()
        {
            // Create ErrorMessage
            var message = Error.EndlessLoop(null, null, null, "1");

            var expected = new ValidationResult
            {
                ErrorId = 1,
                FullId = "1.24.1",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Certain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Endless loop detected. Involved items '1'",
                HowToFix = String.Empty,
                HasCodeFix = false
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }

        [TestMethod]
        public void Protocol_CheckEndlessLoop_PotentialEndlessLoop()
        {
            // Create ErrorMessage
            var message = Error.PotentialEndlessLoop(null, null, null, "1");

            var expected = new ValidationResult
            {
                ErrorId = 2,
                FullId = "1.24.2",
                Category = Category.Protocol,
                Severity = Severity.Critical,
                Certainty = Certainty.Uncertain,
                Source = Source.Validator,
                FixImpact = FixImpact.Breaking,
                GroupDescription = String.Empty,
                Description = "Potential endless loop detected. Involved items '1'",
                HowToFix = String.Empty,
                HasCodeFix = false,
            };

            // Assert
            message.Should().BeEquivalentTo(expected, Generic.ExcludePropertiesForErrorMessages);
        }
    }

    [TestClass]

    public class Attribute
    {
        private readonly IRoot check = new CheckEndlessLoop();

        [TestMethod]
        public void Protocol_CheckEndlessLoop_CheckCategory() => Generic.CheckCategory(check, Category.Protocol);

        [TestMethod]
        public void Protocol_CheckEndlessLoop_CheckId() => Generic.CheckId(check, CheckId.CheckEndlessLoop);
    }
}