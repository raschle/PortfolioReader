﻿using System.Collections;
using Toqe.PortfolioReader.Business.Protobuf;
using Xunit;

namespace Toqe.PortfolioReader.Test.Protobuf
{
    public class PortfolioProtobufWriterTest
    {
        [Fact]
        public void TestWriteClient()
        {
            var basePath = new TestHelper().GetTestDataBasePath();
            var reader = new PortfolioProtobufReader();
            var readFilename = Path.Combine(basePath.FullName, "test.portfolio");
            var readClient = reader.ReadClient(readFilename);
            var writer = new PortfolioProtobufWriter();
            var writeFilename = Path.Combine(basePath.FullName, "test-written.portfolio");

            if (File.Exists(writeFilename))
            {
                File.Delete(writeFilename);
            }

            writer.WriteClient(writeFilename, readClient);

            var writtenAndReadClient = reader.ReadClient(writeFilename);

            this.Validate(readClient, writtenAndReadClient);
        }

        private void Validate<T>(T expected, T actual)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                var expectedProp = property.GetValue(expected);
                var actualProp = property.GetValue(actual);

                if (property.PropertyType.Namespace.StartsWith("Toqe.PortfolioReader.Business.Protobuf"))
                {
                    this.Validate(expectedProp, actualProp);
                }
                else if (property.PropertyType.IsGenericType &&
                         property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Assert.True((expectedProp == null) == (actualProp == null));

                    if (expectedProp != null)
                    {
                        Assert.Equal(((IList)expectedProp).Count, ((IList)actualProp).Count);

                        for (var i = 0; i < ((IList)expectedProp).Count; i++)
                        {
                            this.Validate(((IList)expectedProp)[i], ((IList)actualProp)[i]);
                        }
                    }
                }
                else if (property.PropertyType.IsGenericType &&
                         property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Assert.True((expectedProp == null) == (actualProp == null));

                    if (expectedProp != null)
                    {
                        Assert.Equal(((IDictionary)expectedProp).Count, ((IDictionary)actualProp).Count);
                        Assert.Equal(((IDictionary)expectedProp).Keys, ((IDictionary)actualProp).Keys);

                        foreach (var key in ((IDictionary)expectedProp).Keys)
                        {
                            this.Validate(((IDictionary)expectedProp)[key], ((IDictionary)actualProp)[key]);
                        }
                    }
                }
                else
                {
                    Assert.Equal(expectedProp, actualProp);
                }
            }
        }
    }
}