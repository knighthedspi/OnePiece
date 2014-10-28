using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QueryCondition<T> {
	
	private List<T>                                        equalConditions;
	private List<T>                                        equalGreaterThanConditions;
	private List<T>                                        equalLessThanConditions;
    private List<T>                                        greaterThanConditions;
	private List<T>                                        lessThanConditions;

	private string[]                                       fields;
	private string[]                                       groups;
	private Dictionary<string ,string>                     orders;
	private int                                            limit     = 1000; 
	private int                                            offset    = 0; 
	private string                                         tablename = typeof(T).Name; 
	

	public QueryCondition<T> WithEqualConditions(List<T> conditions) {
		this.equalConditions = conditions;
		return this;
	}

	public QueryCondition<T> WithEqualGreaterThanConditions(List<T> conditions) {
		this.equalGreaterThanConditions = conditions;
		return this;
    }

	public QueryCondition<T> WithEqualLessThanConditions(List<T> conditions) {
		this.equalLessThanConditions = conditions;
		return this;
    }

	public QueryCondition<T> WithGreaterThanConditions(List<T> conditions) {
		this.greaterThanConditions = conditions;
		return this;
    }

	public QueryCondition<T> WithLessThanConditions(List<T> conditions) {
		this.lessThanConditions = conditions;
		return this;
    }
    
    public QueryCondition<T> WithFields(string[] fields) {
		this.fields = fields;
		return this;
	}

	public QueryCondition<T> WithGroups(string[] groups) {
		this.groups = groups;
		return this;
	}

	public QueryCondition<T> WithOrders(Dictionary<string ,string> orders) {
		this.orders = orders;
		return this;
	}

	public QueryCondition<T> WithLimit(int limit) {
		this.limit = limit;
		return this;
	}

	public QueryCondition<T> WithOffset(int offset) {
		this.offset = offset;
		return this;
	}

	public QueryCondition<T> WithTablename(string tablename) {
		this.tablename = tablename;
		return this;
	}


    public List<T> EqualConditions {
		get {
			return this.equalConditions;
		}
		set {
			equalConditions = value;
		}
	}

	public List<T> EqualGreaterThanConditions {
		get {
			return this.equalGreaterThanConditions;
		}
		set {
			equalGreaterThanConditions = value;
		}
	}

	public List<T> EqualLessThanConditions {
		get {
			return this.equalLessThanConditions;
		}
		set {
			equalLessThanConditions = value;
		}
	}

	public List<T> GreaterThanConditions {
		get {
			return this.greaterThanConditions;
		}
		set {
			greaterThanConditions = value;
		}
	}

	public List<T> LessThanConditions {
		get {
			return this.lessThanConditions;
		}
		set {
			lessThanConditions = value;
		}
	}

	public string[] Fields {
		get {
			return this.fields;
		}
		set {
			fields = value;
		}
	}

	public string[] Groups {
		get {
			return this.groups;
		}
		set {
			groups = value;
		}
	}

	public Dictionary<string, string> Orders {
		get {
			return this.orders;
		}
		set {
			orders = value;
		}
	}

	public int Limit {
		get {
			return this.limit;
		}
		set {
			limit = value;
		}
	}

	public int Offset {
		get {
			return this.offset;
		}
		set {
			offset = value;
		}
	}

	public string Tablename {
		get {
			return this.tablename;
		}
		set {
			tablename = value;
		}
	}
}