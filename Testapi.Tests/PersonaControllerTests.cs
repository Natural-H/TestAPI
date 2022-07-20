// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Api.Controllers;
// using Api.DataModels;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;

// namespace Api.Tests;

// public class PersonaControllerTests
// {
//     public List<Persona> GetTestPersonas()
//         => new List<Persona> {
//             new Persona{ PersonaId = 1, Nombre = "Alberto" },
//             new Persona{ PersonaId = 2, Nombre = "Pedro" },
//         };

//     [Fact]
//     public void TestLogging()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         mockDb.AddRange(GetTestPersonas());
//         mockDb.SaveChanges();
//         var controller = new PersonaController(mockLogger.Object, mockDb);

//         // Act
//         var result = controller.TestLogging();

//         // Assert
//         Assert.Equal(200, result.StatusCode);
//     }

//     [Fact]
//     public async Task TestGetPersonas()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         mockDb.AddRange(GetTestPersonas());
//         mockDb.SaveChanges();
//         var controller = new PersonaController(mockLogger.Object, mockDb);

//         // Act
//         var result = await controller.Get();

//         // Assert
//         var data = Assert.IsAssignableFrom<IEnumerable<Persona>>(result);
//         Assert.Equal(GetTestPersonas().Count(), data.Count());
//         var matchingTuples = GetTestPersonas().Zip(data, (e, r) => new { Expected = e, Reading = r});
//         foreach (var tuple in matchingTuples) {
//             Assert.Equal(tuple.Expected.PersonaId, tuple.Reading.PersonaId);
//             Assert.Equal(tuple.Expected.Nombre, tuple.Reading.Nombre);
//         }
//     }

//     [Fact]
//     public async Task TestGetPersonaById()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         mockDb.AddRange(GetTestPersonas());
//         mockDb.SaveChanges();
//         var controller = new PersonaController(mockLogger.Object, mockDb);
//         var form = GetTestPersonas()[0];

//         // Act
//         var result = await controller.GetById(form.PersonaId);

//         // Assert
//         var persona = Assert.IsType<Persona>(Assert.IsType<ObjectResult>(result).Value);
//         Assert.Equal(form.PersonaId, persona.PersonaId);
//         Assert.Equal(form.Nombre, persona.Nombre);
//     }

//     [Fact]
//     public async Task TestGetPersonaByIdNotFound()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         var controller = new PersonaController(mockLogger.Object, mockDb);

//         // Act
//         var result = await controller.GetById(-1);

//         // Assert
//         var statusCode = Assert.IsType<NotFoundResult>(result).StatusCode;
//         Assert.Equal(404, statusCode);
//     }

//     [Fact]
//     public async Task TestAddPersona()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         var controller = new PersonaController(mockLogger.Object, mockDb);
//         var form = new Persona { PersonaId = 0, Nombre = "Mario" };

//         // Act
//         var result = await controller.AddOrUpdate(form);

//         // Assert
//         var data = Assert.IsAssignableFrom<long>(Assert.IsType<JsonResult>(result).Value);
//         Assert.NotEqual(0, data);
//         var persona = Assert.IsType<Persona>(Assert.IsType<ObjectResult>(await controller.GetById(data)).Value);
//         Assert.Equal(form.PersonaId, persona.PersonaId);
//         Assert.Equal(form.Nombre, persona.Nombre);
//     }

//     [Fact]
//     public async Task TestUpdatePersona()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         mockDb.AddRange(GetTestPersonas());
//         mockDb.SaveChanges();
//         var controller = new PersonaController(mockLogger.Object, mockDb);
//         var form = new Persona { PersonaId = GetTestPersonas()[0].PersonaId, Nombre = "Mario" };

//         // Act
//         var result = await controller.AddOrUpdate(form);

//         // Assert
//         var data = Assert.IsAssignableFrom<long>(Assert.IsType<JsonResult>(result).Value);
//         var persona = Assert.IsType<Persona>(Assert.IsType<ObjectResult>(await controller.GetById(data)).Value);
//         Assert.Equal(form.PersonaId, persona.PersonaId);
//         Assert.Equal(form.Nombre, persona.Nombre);
//     }

//     [Fact]
//     public async Task TestUpdatePersonaNotFound()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         var controller = new PersonaController(mockLogger.Object, mockDb);
//         var form = new Persona { PersonaId = -1, Nombre = "Mario" };

//         // Act
//         var result = await controller.AddOrUpdate(form);

//         // Assert
//         var statusCode = Assert.IsType<NotFoundResult>(result).StatusCode;
//         Assert.Equal(404, statusCode);
//     }

//     [Fact]
//     public async Task TestDeletePersona()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         mockDb.AddRange(GetTestPersonas());
//         mockDb.SaveChanges();
//         var controller = new PersonaController(mockLogger.Object, mockDb);
//         var form = GetTestPersonas()[0];

//         // Act
//         var result = await controller.Delete(form.PersonaId);

//         // Assert
//         Assert.Equal(200, result.StatusCode);
//     }

//     [Fact]
//     public async Task TestDeletePersonaNotFound()
//     {
//         // Arrange
//         var mockLogger = new Mock<ILogger<PersonaController>>();
//         var mockDb = new EjemploDb(new DbContextOptionsBuilder<EjemploDb>()
//             .UseInMemoryDatabase(Guid.NewGuid().ToString())
//             .Options);
//         var controller = new PersonaController(mockLogger.Object, mockDb);

//         // Act
//         var result = await controller.Delete(-1);

//         // Assert
//         var statusCode = Assert.IsType<NotFoundResult>(result).StatusCode;
//         Assert.Equal(404, statusCode);
//     }
// }