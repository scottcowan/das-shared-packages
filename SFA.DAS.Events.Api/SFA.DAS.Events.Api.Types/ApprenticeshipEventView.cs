﻿using System;

namespace SFA.DAS.Events.Api.Types
{
    public class ApprenticeshipEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PaymentStatus { get; set; }
        public long ApprenticeshipId { get; set; }
        public string AgreementStatus { get; set; }
        public string ProviderId { get; set; }
        public string LearnerId { get; set; }
        public string EmployerAccountId { get; set; }
        public TrainingTypes TrainingType { get; set; }
        public string TrainingId { get; set; }
        public DateTime TrainingStartDate { get; set; }
        public DateTime TrainingEndDate { get; set; }
        public decimal TrainingTotalCost { get; set; }
    }
}