// ------------------------------------------------------------------------------
// <auto-generated>
//    Generated by avrogen, version 1.7.7.5
//    Changes to this file may cause incorrect behavior and will be lost if code
//    is regenerated
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Udemy
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using global::Avro;
	using global::Avro.Specific;
	
	public partial class Review : ISpecificRecord
	{
		public static Schema _SCHEMA = Schema.Parse(@"{""type"":""record"",""name"":""Review"",""namespace"":""Udemy"",""fields"":[{""name"":""Id"",""doc"":""Review ID as per Udemy's db"",""type"":""long""},{""name"":""Title"",""default"":null,""type"":[""null"",""string""]},{""name"":""Content"",""doc"":""Review text if provided"",""default"":null,""type"":[""null"",""string""]},{""name"":""Rating"",""doc"":""review value"",""type"":""string""},{""name"":""Created"",""type"":""long""},{""name"":""Modified"",""type"":""long""},{""name"":""User"",""type"":{""type"":""record"",""name"":""User"",""namespace"":""Udemy"",""fields"":[{""name"":""Title"",""type"":""string""},{""name"":""Name"",""doc"":""first name"",""type"":""string""},{""name"":""DisplayName"",""type"":""string""}]}},{""name"":""Course"",""type"":{""type"":""record"",""name"":""Course"",""namespace"":""Udemy"",""fields"":[{""name"":""Id"",""doc"":""Course ID in Udemy's DB"",""type"":""long""},{""name"":""Title"",""type"":""string""},{""name"":""Url"",""type"":""string""}]}}]}");
		/// <summary>
		/// Review ID as per Udemy's db
		/// </summary>
		private long _Id;
		private string _Title;
		/// <summary>
		/// Review text if provided
		/// </summary>
		private string _Content;
		/// <summary>
		/// review value
		/// </summary>
		private string _Rating;
		private long _Created;
		private long _Modified;
		private Udemy.User _User;
		private Udemy.Course _Course;
		public virtual Schema Schema
		{
			get
			{
				return Review._SCHEMA;
			}
		}
		/// <summary>
		/// Review ID as per Udemy's db
		/// </summary>
		public long Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				this._Id = value;
			}
		}
		public string Title
		{
			get
			{
				return this._Title;
			}
			set
			{
				this._Title = value;
			}
		}
		/// <summary>
		/// Review text if provided
		/// </summary>
		public string Content
		{
			get
			{
				return this._Content;
			}
			set
			{
				this._Content = value;
			}
		}
		/// <summary>
		/// review value
		/// </summary>
		public string Rating
		{
			get
			{
				return this._Rating;
			}
			set
			{
				this._Rating = value;
			}
		}
		public long Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				this._Created = value;
			}
		}
		public long Modified
		{
			get
			{
				return this._Modified;
			}
			set
			{
				this._Modified = value;
			}
		}
		public Udemy.User User
		{
			get
			{
				return this._User;
			}
			set
			{
				this._User = value;
			}
		}
		public Udemy.Course Course
		{
			get
			{
				return this._Course;
			}
			set
			{
				this._Course = value;
			}
		}
		public virtual object Get(int fieldPos)
		{
			switch (fieldPos)
			{
			case 0: return this.Id;
			case 1: return this.Title;
			case 2: return this.Content;
			case 3: return this.Rating;
			case 4: return this.Created;
			case 5: return this.Modified;
			case 6: return this.User;
			case 7: return this.Course;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
			};
		}
		public virtual void Put(int fieldPos, object fieldValue)
		{
			switch (fieldPos)
			{
			case 0: this.Id = (System.Int64)fieldValue; break;
			case 1: this.Title = (System.String)fieldValue; break;
			case 2: this.Content = (System.String)fieldValue; break;
			case 3: this.Rating = (System.String)fieldValue; break;
			case 4: this.Created = (System.Int64)fieldValue; break;
			case 5: this.Modified = (System.Int64)fieldValue; break;
			case 6: this.User = (Udemy.User)fieldValue; break;
			case 7: this.Course = (Udemy.Course)fieldValue; break;
			default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
			};
		}
	}
}