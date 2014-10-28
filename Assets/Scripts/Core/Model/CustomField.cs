using System;

[AttributeUsage(AttributeTargets.Property | 
                AttributeTargets.Field)]
public class CustomField : Attribute{
	public string Statement { get; set; }
}
