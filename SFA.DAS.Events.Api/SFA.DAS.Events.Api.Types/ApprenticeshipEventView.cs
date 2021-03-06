﻿using System;

namespace SFA.DAS.Events.Api.Types
{
    public class ApprenticeshipEventView : IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public DateTime CreatedOn { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public long ApprenticeshipId { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public string ProviderId { get; set; }
        public string LearnerId { get; set; }
        public string EmployerAccountId { get; set; }
        public TrainingTypes TrainingType { get; set; }
        public string TrainingId { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public DateTime? TrainingEndDate { get; set; }
        public decimal? TrainingTotalCost { get; set; }
        public int PaymentOrder { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityOrganisationType { get; set; }
        public string Type => this.GetType().Name;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}