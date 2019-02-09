# Gobln.Pager

Gobln.Pager is a .Net easy to use pager library written in C#, to allow you covert IEnumarble/IQueryable to an PageList or Page object.
And can be usable in any type of project.

If you would like to use Gobln.Pager in Mvc, check the following link [Gobln.Pager.Mvc](https://nuget.org/packages/Gobln.Pager.Mvc).

## Frameworks

* .Net 4.0 and higher
* .Net Core 1.0 and higher
* .Net Core 2.0 and higher
* .Net Standard 1.6 and higher

## Page

### How to use

Install Gobln.Pager, trough [Nuget](https://nuget.org/) or other means.
Use the extension .ToPage() on your IEnumarble/IQueryable and you will get the 10 first items from you IEnumarble/IQueryable.
To Change the page size or the selected page you onlu need to change the first two value of ToPager({page index}, {page size})

### Examples

```csharp

// Create an List oject
var list = new List<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
                new TestModel1(){ Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
                new TestModel1(){ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
                new TestModel1(){ Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
                new TestModel1(){ Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
                new TestModel1(){ Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
            };

// Create an Page object
//  this will get you the first page index with the 10 first items
var page = list.ToPage();

// Create an Page object with pagesize 2 and pageindex 3
page = list.ToPage(3, 2);

// Create an Page object from a prepaged list where that the pagesize 10, pageindex 10 and the total item count 100
page = list.ToPage(5, 10, 100, prePaged: true);

// Use PageFilter of IPageFilter
var pagerFilter = new PagerFilter()
    {
        PageIndex = 5,
        PageSize = 2
    };

var page = testList.ToPage(pagerFilter);

```

For more examples, check the test project

## PagedList

### How to use

The PagedList object contains information that will go to make up your Page object.
This will reprisent the full list of items that from wich you can easly add or remove items, and check the content of the page.
Or even cycle trought the pages.  

### Examples

```csharp
// Create an pagedList object
var pageList = new PagedList<TestModel1>()
            {
                new TestModel1(){ Id = 1, Name = "Tester1", Date = new DateTime( 2016, 5,1 ) },
                new TestModel1(){ Id = 2, Name = "Tester2", Date = new DateTime( 2016, 5,2 ) },
                new TestModel1(){ Id = 3, Name = "Tester3", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 4, Name = "Tester4", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 5, Name = "Tester5", Date = new DateTime( 2016, 5,5 ) },
                new TestModel1(){ Id = 6, Name = "Tester6", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 7, Name = "Tester7", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 8, Name = "Tester8", Date = new DateTime( 2016, 5,5 ) },
                new TestModel1(){ Id = 9, Name = "Tester9", Date = new DateTime( 2016, 5,5 ) },
                new TestModel1(){ Id = 10, Name = "Tester10", Date = new DateTime( 2016, 5,3 ) },
                new TestModel1(){ Id = 11, Name = "Tester11", Date = new DateTime( 2016, 5,4 ) },
                new TestModel1(){ Id = 12, Name = "Tester12", Date = new DateTime( 2016, 5,5 ) },
            };

// Add extra item
pageList.Add(new TestModel1() { Id = 16, Name = "Tester16", Date = new DateTime(2015, 5, 5) });

// Set the page values, if not set default pageidex is 1 and size is 10
pageList.CurrentPageIndex = 2;
pageList.PageSize = 3;

// Get the current page form the pagelist
var pager = pageList.GetCurrentPage();

// Get the next page
pager = pageList.GetNextPage();

// Get the page at index X
pager = pageList.GetPage(1);

```

For more examples, check the test project

## Installing Gobln.Pager

The project is on [Nuget](https://www.nuget.org/packages/Gobln.Pager/). Install via the NuGet Package Manager.

PM > Install-Package Gobln.Pager

## License

[Apache License, Version 2.0](http://opensource.org/licenses/Apache-2.0).

## Documentation and Readme file

I'm going to provide an documentation file, but haven't started on one yet.
As for the Readme file, if there are any inconsitencies or grammatical errors feel free to let me know by an pull request. This also counts for problems in de code.

## Issues and Contributions

* If something is broken and you know how to fix it, send a pull request.
* If you have no idea what is wrong, create an issue.

## Any feedback and contributions are welcome

If you have something you'd like to improve do not hesitate to send a Pull Request