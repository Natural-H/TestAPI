namespace Testapi.Tests;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using Xunit;
using Testapi.DataTranserObjects;
using Testapi.Models;
using Testapi.Mappings;
using Testapi.Controllers;
using AutoMapper;

public class UnitTest1
{
    [Fact]
    public async Task PostPersonTest()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        var form = new PersonDTO()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    address = "Address1"
                },
                new() {
                    address = "Address2"
                }
            }
        };

        // Act
        var Result = await PeopleController.PostPerson(form);

        // Assert
        var Person = Assert.IsType<PersonDTO>(Assert.IsType<ActionResult<PersonDTO>>(await PeopleController.GetPerson(form.Id)).Value);
        Assert.IsType<ActionResult<PersonDTO>>(Result);

        Assert.Equal(form.Id, Person.Id);
        Assert.Equal(form.Name, Person.Name);
        Assert.Equal(form.Addresses, Person.Addresses);
    }

    [Fact]
    public async Task PostAddressTest()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        await PeopleController.PostPerson(new PersonDTO()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Address2"
                }
            }
        });

        var form = new Address()
        {
            Id = 3,
            PersonId = 1,
            address = "New Address"
        };

        // Act
        var Result = await PeopleController.PostAddress(1, form);

        // Assert
        var NewAddress = Assert.IsType<Address>(Assert.IsType<ActionResult<Address>>(await PeopleController.GetAddress(form.Id)).Value);
        Assert.IsType<ActionResult<Address>>(Result);

        Assert.Equal(form.Id, NewAddress.Id);
        Assert.Equal(form.PersonId, NewAddress.PersonId);
        Assert.Equal(form.address, NewAddress.address);
    }

    [Fact]
    public async Task UpdatePersonTest()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        await PeopleController.PostPerson(new()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Address2"
                }
            }
        });

        // Act
        var UpdatedPerson = new PersonDTO()
        {
            Id = 1,
            Name = "John Updated Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Updated Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Updated Address2"
                }
            }
        };

        var Result = await PeopleController.PutPerson(UpdatedPerson.Id, UpdatedPerson);

        // Assert
        var Person = Assert.IsType<PersonDTO>(Assert.IsType<ActionResult<PersonDTO>>(await PeopleController.GetPerson(UpdatedPerson.Id)).Value);
        Assert.IsType<NoContentResult>(Result);

        Assert.Equal(UpdatedPerson.Id, Person.Id);
        Assert.Equal(UpdatedPerson.Name, Person.Name);
        Assert.Equal(UpdatedPerson.Addresses, Person.Addresses);
    }

    [Fact]
    public async Task UpdatePersonNotFound()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        // Act
        var UpdatedPerson = new PersonDTO()
        {
            Id = 1,
            Name = "John Updated Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Updated Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Updated Address2"
                }
            }
        };

        var Result = await PeopleController.PutPerson(UpdatedPerson.Id, UpdatedPerson);

        // Assert
        var ActualStatusCode = Assert.IsType<NotFoundResult>(Result).StatusCode;
        var Expected = StatusCodes.Status404NotFound;

        Assert.Equal(Expected, ActualStatusCode);
    }

    [Fact]
    public async Task DeletePersonTest()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        var Person = new PersonDTO()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Address2"
                }
            }
        };

        await PeopleController.PostPerson(Person);
    
        // Act
        var Result = await PeopleController.DeletePerson(Person.Id);
    
        // Assert
        var ActualStatusCode = Assert.IsType<NoContentResult>(Result).StatusCode;
        var Excpected = StatusCodes.Status204NoContent;

        Assert.Equal(Excpected, ActualStatusCode);
    }

    [Fact]
    public async Task DeletePersonNotFound()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        var Person = new PersonDTO()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Address2"
                }
            }
        };
    
        // Act
        var Result = await PeopleController.DeletePerson(Person.Id);
    
        // Assert
        var ActualStatusCode = Assert.IsType<NotFoundResult>(Result).StatusCode;
        var Excpected = StatusCodes.Status404NotFound;

        Assert.Equal(Excpected, ActualStatusCode);
    }

    [Fact]
    public async Task DeleteAddressTest()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        var Person = new PersonDTO()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Address2"
                }
            }
        };

        var Address = new Address()
        {
            Id = 3,
            PersonId = 1,
            address = "Address3"
        };

        await PeopleController.PostPerson(Person);
        await PeopleController.PostAddress(Person.Id, Address);
    
        // Act
        var Result = await PeopleController.DeleteAddress(Person.Id, Address.Id);
    
        // Assert
        var ActualStatusCode = Assert.IsType<NoContentResult>(Result).StatusCode;
        var Excpected = StatusCodes.Status204NoContent;

        Assert.Equal(Excpected, ActualStatusCode);
    }

    [Fact]
    public async Task DeleteAddressNotFound()
    {
        // Arrage
        var DbContext = new PeopleContext();
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();

        var PeopleController = new PeopleController(
            context: DbContext,
            mapper: new Mapper(
                configurationProvider: new MapperConfiguration(
                    cfg => cfg.AddProfile(new AutoMapperProfile())
                )
            )
        );

        var Person = new PersonDTO()
        {
            Id = 1,
            Name = "John Doe",
            Addresses = {
                new() {
                    Id = 1,
                    PersonId = 1,
                    address = "Address1"
                },
                new() {
                    Id = 2,
                    PersonId = 1,
                    address = "Address2"
                }
            }
        };

        var Address = new Address()
        {
            Id = 3,
            PersonId = 1,
            address = "Address3"
        };

        await PeopleController.PostPerson(Person);
    
        // Act
        var Result = await PeopleController.DeleteAddress(Person.Id, Address.Id);
    
        // Assert
        var ActualStatusCode = Assert.IsType<NotFoundResult>(Result).StatusCode;
        var Excpected = StatusCodes.Status404NotFound;

        Assert.Equal(Excpected, ActualStatusCode);
    }
}