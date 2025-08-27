using System;

[Serializable]
public class FirestoreList
{
    public Entry[] entries;
}

[Serializable]
public class Entry
{
    public Document document;
}

[Serializable]
public class Document
{
    public Fields fields;
}

[Serializable]
public class Fields
{
    public StringValue name;
    public IntValue wave;
    public IntValue killed;
    public IntValue revive;
}

[Serializable]
public class StringValue
{
    public string stringValue;
}

[Serializable]
public class IntValue
{
    public string integerValue;
}

[Serializable]
public class AggregationResponseList
{
    public AggregationResponse[] items;
}

[Serializable]
public class AggregationResponse
{
    public Result result;
}
[Serializable]
public class Result
{
    public AggregateFields aggregateFields;
}
[Serializable]
public class AggregateFields
{
    public IntValue field_1;
}