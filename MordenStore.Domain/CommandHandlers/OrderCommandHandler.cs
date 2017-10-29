﻿using FluentValidator;
using ModernStore.Share.Commands;
using MordenStore.Domain.Commands;
using MordenStore.Domain.Entities;
using MordenStore.Domain.Repositories;
using System;

namespace MordenStore.Domain.CommandHandlers
{
    public class OrderCommandHandler : Notifiable, ICommandHandler<RegisterOrderCommand>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderCommandHandler(ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public void Handle(RegisterOrderCommand command)
        {
            // Instacia o repositorio
            var customer = _customerRepository.GetByUserId(command.Customer);

            // Gera um novo pedido
            var order = new Order(customer, command.DeliveryFee, command.Discount);

            // Adiciona os itens no pedido
            foreach (var item in command.Items)
            {
                var product = _productRepository.Get(item.Product);
                order.AddItem(new OrderItem(product, item.Quantity));
            }

            // Adiciona as notificações do pedido no handler
            AddNotifications(order.Notifications);

            // Persiste no banco
            if (order.IsValid())
                _orderRepository.Save(order);
        }
    }
}