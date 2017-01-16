﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiAgreementsTests
{
    [TestFixture]
    public class WhenIGetAgreementEventsById : EventsApiTestBase
    {
        [Test]
        public async Task ThenAgreementEventsAreReturned()
        {
            var fromEventId = 123;
            var pageSize = 500;
            var pageNumber = 3;

            var url = $"{BaseUrl}api/events/engagements?fromEventId={fromEventId}&pageSize={pageSize}&pageNumber={pageNumber}";

            var expectedEvents = new List<AgreementEventView> { new AgreementEventView { CreatedOn = DateTime.Now.AddDays(-1), EmployerAccountId = "ABC123", Event = "Test", Id = 87435, ProviderId = "ZZZ999" } };

            SecureHttpClient.Setup(x => x.GetAsync(url, ClientToken)).ReturnsAsync(JsonConvert.SerializeObject(expectedEvents));

            var response = await Api.GetAgreementEventsById(fromEventId, pageSize, pageNumber);

            response.ShouldBeEquivalentTo(expectedEvents);
        }
    }
}
