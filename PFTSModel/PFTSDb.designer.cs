﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PFTSModel
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="pfts")]
	public partial class PFTSDbDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 可扩展性方法定义
    partial void OnCreated();
    partial void InsertOfficer(Officer instance);
    partial void UpdateOfficer(Officer instance);
    partial void DeleteOfficer(Officer instance);
    partial void InsertOperator(Operator instance);
    partial void UpdateOperator(Operator instance);
    partial void DeleteOperator(Operator instance);
    #endregion
		
		public PFTSDbDataContext() : 
				base(global::PFTSModel.Properties.Settings.Default.pftsConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public PFTSDbDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PFTSDbDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PFTSDbDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PFTSDbDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Officer> Officer
		{
			get
			{
				return this.GetTable<Officer>();
			}
		}
		
		public System.Data.Linq.Table<Operator> Operator
		{
			get
			{
				return this.GetTable<Operator>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.officer")]
	public partial class Officer : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _no;
		
		private string _name;
		
		private string _sex;
		
		private System.Data.Linq.Binary _fingerprint;
		
		private System.Nullable<System.DateTime> _create_time;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnnoChanging(string value);
    partial void OnnoChanged();
    partial void OnnameChanging(string value);
    partial void OnnameChanged();
    partial void OnsexChanging(string value);
    partial void OnsexChanged();
    partial void OnfingerprintChanging(System.Data.Linq.Binary value);
    partial void OnfingerprintChanged();
    partial void Oncreate_timeChanging(System.Nullable<System.DateTime> value);
    partial void Oncreate_timeChanged();
    #endregion
		
		public Officer()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_no", DbType="NChar(32)")]
		public string no
		{
			get
			{
				return this._no;
			}
			set
			{
				if ((this._no != value))
				{
					this.OnnoChanging(value);
					this.SendPropertyChanging();
					this._no = value;
					this.SendPropertyChanged("no");
					this.OnnoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_name", DbType="NChar(32) NOT NULL", CanBeNull=false)]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if ((this._name != value))
				{
					this.OnnameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("name");
					this.OnnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_sex", DbType="NChar(8)")]
		public string sex
		{
			get
			{
				return this._sex;
			}
			set
			{
				if ((this._sex != value))
				{
					this.OnsexChanging(value);
					this.SendPropertyChanging();
					this._sex = value;
					this.SendPropertyChanged("sex");
					this.OnsexChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_fingerprint", DbType="Binary(2048)", UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary fingerprint
		{
			get
			{
				return this._fingerprint;
			}
			set
			{
				if ((this._fingerprint != value))
				{
					this.OnfingerprintChanging(value);
					this.SendPropertyChanging();
					this._fingerprint = value;
					this.SendPropertyChanged("fingerprint");
					this.OnfingerprintChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_create_time", DbType="DateTime")]
		public System.Nullable<System.DateTime> create_time
		{
			get
			{
				return this._create_time;
			}
			set
			{
				if ((this._create_time != value))
				{
					this.Oncreate_timeChanging(value);
					this.SendPropertyChanging();
					this._create_time = value;
					this.SendPropertyChanged("create_time");
					this.Oncreate_timeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.operator")]
	public partial class Operator : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _account;
		
		private string _password;
		
		private string _name;
		
		private System.Nullable<System.DateTime> _last_login_time;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnaccountChanging(string value);
    partial void OnaccountChanged();
    partial void OnpasswordChanging(string value);
    partial void OnpasswordChanged();
    partial void OnnameChanging(string value);
    partial void OnnameChanged();
    partial void Onlast_login_timeChanging(System.Nullable<System.DateTime> value);
    partial void Onlast_login_timeChanged();
    #endregion
		
		public Operator()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_account", DbType="NChar(32) NOT NULL", CanBeNull=false)]
		public string account
		{
			get
			{
				return this._account;
			}
			set
			{
				if ((this._account != value))
				{
					this.OnaccountChanging(value);
					this.SendPropertyChanging();
					this._account = value;
					this.SendPropertyChanged("account");
					this.OnaccountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_password", DbType="NChar(36) NOT NULL", CanBeNull=false)]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				if ((this._password != value))
				{
					this.OnpasswordChanging(value);
					this.SendPropertyChanging();
					this._password = value;
					this.SendPropertyChanged("password");
					this.OnpasswordChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_name", DbType="NChar(32) NOT NULL", CanBeNull=false)]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if ((this._name != value))
				{
					this.OnnameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("name");
					this.OnnameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_last_login_time", DbType="DateTime")]
		public System.Nullable<System.DateTime> last_login_time
		{
			get
			{
				return this._last_login_time;
			}
			set
			{
				if ((this._last_login_time != value))
				{
					this.Onlast_login_timeChanging(value);
					this.SendPropertyChanging();
					this._last_login_time = value;
					this.SendPropertyChanged("last_login_time");
					this.Onlast_login_timeChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
