# Globals-EF
LinQ API to work with Caché Globals from .NET Entity Framwork

##Introduction
“Globals EF” is an Object/Relational Mapping (O/RM) framework for well-known database Intersystems Caché. As you know, the Caché database is powered by an extremely efficient multidimensional data engine. The exposed interface support access to the multidimensional structures, providing the highest performance and greatest range of storage possibilities. 
Caché is very useful for applications that work with “big data” and there are more and more these applications nowadays. .Net framework with C# programming language makes creating and maintenance of applications very easy for developers. Therefore, easy using Intersystems Caché in .Net applications is actual task.
Intersystems provides Caché eXTreme technology that gives access to Globals API from .Net applications but creating applications is still time-consuming. 
So, main purpose of Globals EF is to ease working with Caché and GlobalsDB. Core of this framework is InterSystems Caché eXTreme technology. Described framework provides a lot of useful functions, such as Object/Relational Mapping, LINQ to Globals and makes interaction with Caché and GlobalsDB simple and seamless. 

##Installation
1. Download Caché for official web - site.
2. Set environment variables:  
  2.1 GLOBALS_HOME - set the path to the Caché installation folder (C:\InterSystems\Cache\ - as example)  
  2.2 PATH - set the path to the Bin direcory (C:\InterSystems\Cache\Bin - as example)
3. After downloading and successful installation of Caché, open System Management Portal and create user, which will be used by Globals EF. For example, TestUser with password - testpassword and namespace - SAMPLES. Note, that service %Service_CallIn must be enabled (System -> Security Management -> Services).
4. Create new project in Visual Studio. Note, that minumum supported version of Visual Studio - is VS2012. 
5. Add reference to the GlobalsFramework.dll assembly
6. Set target platform of your newly created project to the x64. Project-> <Project-name> Properties->Build->Platform target
7. It's all! GlobalsEF is ready for use.

##Public API

####DbSet<TEntity> class

`DbSet<TEntity>` class represents an entity that is used for create, read, update, and delete operations.  
This class implements `IOrderedQueryable<TEntity>` and `IQueryProvider` interfaces, therefore you can use LINQ expressions on `DbSet<TEntity>` instance for obtaining data from database. Due to implementation of `IOrderedQueryable<TEntity>`, you can also use sorting queries, such as OrderBy, OrderByDescending, ThenBy or ThenByDescending.  

Class DbSet<TEntity> also implements `IDbSet<TEntity>` interface which declares following methods:
```c#
    void InsertOnSubmit(TEntity entity); 
    void InsertAllOnSubmit(IEnumerable<TEntity> entities);  
    void UpdateOnSubmit(TEntity entity);  
    void UpdateAllOnSubmit(IEnumerable<TEntity> entities);  
    void DeleteOnSubmit(TEntity entity);  
    void DeleteAllOnSubmit(IEnumerable<TEntity> entities);  
```
These methods can be used for performing corresponding operations with data. You can manipulate single entity or set of entities.

####TEntity

Generic parameter for previously described class (`DbSet<TEntity>`) represents entity which you need to store in a database. 
Restriction for the TEntity – it must be a class, declared as public.  
In this class you can declare public properties which correspond to the entity.   
Note that in database stored only those properties, which meet following requirements:  

1. Property declared as public property (Auto-Implemented property).  
2. ColumnAttribute applied to the property.

####ColumnAttribute

`ColumnAttribute` can be applied to the property of entity to indicate, that this property must be stored in a database.  
`ColumnAttribute` contains following properties:
```c#
    public string Name { get; set; }  
```
Indicate that property of entity (column) must be stored in a database with another name, shorter, of example. If value is not provided, than used name of property in entity class. For example, in this case
```c#
[Column(Name = "Id")]  
public int IdentificationNumber { get; set; }  
```
Property will be stored as “Id” and in this case  
```c#
[Column(Name = "Id")]  
public int IdentificationNumber { get; set; }  
```
as “IdentificationNumber”.
```c#
   public bool IsPrimaryKey { get; set; }
```
Indicate that column is primary key. You can describe complex primary keys by applying `ColumnAttribute` (with `IsPrimaryKey` property set to true) to several columns. Framework manages primary keys and will throw `GlobalsDbException` ("Violation of Primary Key constraint. Cannot insert duplicate key") if you attempt to insert entities with same primary keys.
```c#
   public bool IsDbGenerated { get; set; }
```
Indicate that primary key column is identity, i.e. generated by framework. It supportes only integer types (`Int16`, `Int32` and `Int64`).

There are some rules for applying `ColumnAttribute` in Entities:

1.	Entity must declare at least one primary key with `IsPrimaryKey` property of `ColumnAttribute`
2.	Only integral types (`short`, `int` or `long`) supported as `DbGenerated` column
3.	Only one `DbGenerated` column is allowed
4.	Either `DbGenerated` primary key or custom primary keys are allowed

If these rules are not met, `EntityValidationException` will be thrown.
If entity has identity key, framework will generate it, insert entity to database and set its value to column that marked as identity key.

####DbSetAttribute

DbSetAttribute can be applied to the TEntity class. This attribute describes only one property
```c#
   public string Name { get; set; }
```
Indicate that global for entity in Globals database must have another name. Usage similar to usage same property in ColumnAttribute.

Applying `DbSetAttribute` to TEntity class is not compulsory. 

####DataContext

Abstract class that provides connection to database. You need to create derived class with set of `DbSet<TEntity>` public properties or fields for getting access to database data.   
Declared public method `SubmitChanges()` submits all changes with database that uses have made with public methods of `DbSet<TEntity>` class (such as `InsertOnSubmit(TEntity entity)`).   
`DataContext` class has two protected constructors.  
1. `protected DataContext(string namespc, string user, string password)`  
Takes security parameters for connection to database.  
2. `protected DataContext()`  
Parameterless constructor. It is used for connection to Caché with minimum security level (without user checking) or for connection to GlobalsDB.

####GlobalsDbException

Framework throws the GlobalsDbException in the following cases:

1.	Customer is trying to update entity that does not exist in the database
2.	Customer is trying to delete entity that does not exist in the database
3.	Customer is trying to insert entity with duplicate primary key

####EntityValidationException

Framework throws `EntityValidationException` when customer has declared entity that does not meet following requirements:

1.	Entity must declare at least one primary key with `IsPrimaryKey` property of `ColumnAttribute`
2.	Only integral types (`short`, `int` or `long`) supported as `DbGenerated` column
3.	Only one `DbGenerated` column is allowed
4.	Either `DbGenerated` primary key or custom primary keys are allowed
5.	TEntity class must be declared as public

##How to use

Primarily, your need to declare domain model's classes. In this example, domain model consists of four entities (Country, Faculty, University and Town) which forms complex hierarchy. Every property in the entity, which you need to be stored in the database must be marked with the `ColumnAttrubute`.   
Theses entities have been declared in that way:
```c#
public sealed class Country
{
    [Column(IsPrimaryKey = true)]
    public string Name { get; set; }

    [Column()]
    public Continent Continent { get; set; }

    [Column()]
    public bool HasSee { get; set; }

    [Column()]
    public List<Town> Towns { get; set; }
}
```
```c#
public sealed class Town
{
    [Column()]
    public string Name { get; set; }

    [Column()]
    public bool IsCapital { get; set; }

    [Column()]
    public int Population { get; set; }

    [Column()]
    public List<University> Universities { get; set; }
}
```
```c#
public sealed class University
{
    [Column]
    public string Name { get; set; }

    [Column]
    public string Description { get; set; }

    [Column]
    public List<Faculty> Faculties { get; set; }
}
```
```c#
public class Faculty
{
    [Column()]
    public string Name { get; set; }

    [Column]
    public string Description { get; set; }
}
```
As you can see, every country contains list of towns, every towns contains list of universities and every university contains list of faculties. `ColumnAttribute` is supplied for every column which need to be stored in the database.   
I am going to store Country entity in the database and all child entities will be serialized to globals in the Caché database automatically. According to the rules of `ColumnAttribute`, I need to declare primary key for entity Country. So I marked property `Name` as primary key.   

To operate with Country entity you need, at first, to create derived class of `DataContext`. Secondly, you need to declare `DbSet<TEntity>` property in the derived class, where TEntity – type of entity which you desire to store in the database.
I have declared `TestDataContext` class according to the mentioned rules. Class `TestDataContext` has only one public property `Countries`.  
Also I have declared public parameterless constuctor which calls constructor with security credentials, thus using of UniversityInfoDataContext becomes very convenient.
```c#
internal class UniversityInfoDataContext : DataContext
{   
    public TestDataContext() : base("SAMPLES", "TestUser", "testpassword") { }
    
    public DbSet<Country> Countries { get; set; }
}
```
To manipulate data you can use different methods of `DbSet<TEntity>` which have been described in previous section. Note, that every changes applied only when `SubmitChanges` method of context have been invoked.  
As example, creating new Country entity in the database is very simple:
```c#
internal static void InsertCountry(Country entity)
{
    using (var context = new UniversityInfoDataContext())
    {
        context.Countries.InsertOnSubmit(entity);
        context.SubmitChanges();
    }
}
```
Also, you can use LINQ queries to obtain data from database. For example, you can get all countries which have more than one town:
```c#
internal static List<Country> InsertCountry(Country entity)
{
    using (var context = new UniversityInfoDataContext())
    {
        return context.Countries.Where(country => country.Towns.Count > 1).ToList();
    }
}
```



