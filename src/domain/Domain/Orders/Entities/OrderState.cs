using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace Domain.Orders.Entities
{
    public enum OrderStateEnum
    {
        Created = 0,
        PaymentStarted = 1,
        PaymentInProgress = 2,
        PaymentSucceded = 3,
        PaymentCanceled = 4,
        PaymentError = 5
    }

    public class OrderState : ValueObject
    {
        public OrderStateEnum Value { get; }
        public DateTime? PaymentStartDate { get; }
        public DateTime? PaymentInProgressDate { get; }
        public DateTime? PaymentSuccededDate { get; }
        public DateTime? PaymentCanceledDate { get; }
        public DateTime? PaymentErrorDate { get; }

        protected OrderState() { }
        private OrderState(OrderStateEnum value, DateTime? paymentStartDate = null,
            DateTime? paymentInProgressDate = null, DateTime? paymentSuccededDate = null,
            DateTime? paymentCanceledDate = null, DateTime? paymentErrorDate = null)
        {
            Value = value;
            PaymentStartDate = paymentStartDate;
            PaymentInProgressDate = paymentInProgressDate;
            PaymentSuccededDate = paymentSuccededDate;
            PaymentCanceledDate = paymentCanceledDate;
            PaymentErrorDate = paymentErrorDate;
        }

        public static OrderState PaymentNew => new(OrderStateEnum.Created);

        public static OrderState OrderSubmitted(OrderState previousState, DateTime paymentStartDate)
        {
            if (previousState.Value != OrderStateEnum.Created)
                throw new InvalidOperationException();

            return new OrderState(OrderStateEnum.PaymentStarted, paymentStartDate: paymentStartDate);
        }

        public static OrderState PaymentInProgress(OrderState previousState, DateTime paymentInProgressDate)
        {
            if (previousState.Value != OrderStateEnum.PaymentStarted)
                throw new InvalidOperationException();

            return new OrderState(OrderStateEnum.PaymentInProgress,
                paymentStartDate: previousState.PaymentStartDate,
                paymentInProgressDate: paymentInProgressDate);
        }

        public static OrderState PaymentSucceded(OrderState previousState, DateTime paymentSuccededDate)
        {
            if (previousState.Value != OrderStateEnum.PaymentInProgress)
                throw new InvalidOperationException();

            return new OrderState(OrderStateEnum.PaymentInProgress,
                paymentStartDate: previousState.PaymentStartDate,
                paymentInProgressDate: previousState.PaymentInProgressDate,
                paymentSuccededDate: paymentSuccededDate);
        }

        public static OrderState PaymentError(OrderState previousState, DateTime paymentErrorDate)
        {
            if (previousState.Value == OrderStateEnum.PaymentError || previousState.Value == OrderStateEnum.PaymentCanceled)
                throw new InvalidOperationException();

            return new OrderState(OrderStateEnum.PaymentError,
                paymentStartDate: previousState.PaymentStartDate,
                paymentInProgressDate: previousState.PaymentInProgressDate,
                paymentSuccededDate: previousState.PaymentSuccededDate,
                paymentErrorDate: paymentErrorDate);
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return PaymentStartDate;
            yield return PaymentInProgressDate;
            yield return PaymentSuccededDate;
            yield return PaymentCanceledDate;
            yield return PaymentErrorDate;
        }
    }
}
