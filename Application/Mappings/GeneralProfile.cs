using Application.Features.Products.Commands.CreateProduct;
using Application.Features.Products.Queries.GetAllProducts;
using Application.Features.Supplier.Commands;
using Application.Features.Supplier.Queries;
using Application.Features.Inventory.Commands;
using Application.Features.Inventory.Queries;
using Application.Features.Acquisition.Commands;
using Application.Features.Acquisition.Queries;
using Application.DTOs.Supplier;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Features.Users.Commands;
using Application.DTOs.Inventory;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.DTOs.Acquisition;
using Application.DTOs.Orders;
using Application.Features.Inbound.Commands;
using Application.Features.Inbound.Queries;
using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using Application.Features.Outbound.Queries;
using Application.Features.Outbound.Commands;


namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            //Product
            CreateMap<Product, GetAllProductsViewModel>().ReverseMap();
            CreateMap<CreateProductCommand, Product>();
            CreateMap<GetAllProductsQuery, GetAllProductsParameter>();

            //Supplier
            CreateMap<Supplier, SupplierResponseDto>();
            CreateMap<CreateSupplierCommand, Supplier>();
            CreateMap<UpdateSupplierCommand, Supplier>();
            CreateMap<GetAllSupplierQuery, GetAllSupplierParameter>();

            //Inventory
            CreateMap<Inventory, InventoryResponseDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));
            CreateMap<CreateInventoryCommand, Inventory>();
            CreateMap<UpdateInventoryCommand, Inventory>();
            CreateMap<GetAllInventoryQuery, GetAllInventoryParameter>();

            //Inbound
            CreateMap<Inbound, InboundResponseDto>()
                 .ForMember(dest => dest.Inventory, opt => opt.MapFrom(src => src.Inventory));
            CreateMap<CreateInboundCommand, Inbound>();
            CreateMap<GetAllInboundQuery, GetAllInboundParameter>();

            //Outbound
            CreateMap<Outbound, OutboundResponseDto>()
                 .ForMember(dest => dest.Inventory, opt => opt.MapFrom(src => src.Inventory))
                 .ForMember(dest => dest.Inbound, opt => opt.MapFrom(src => src.Inbound));
            CreateMap<GetAllOutboundQuery, GetAllOutboundParameter>();

            //Acquisition
            CreateMap<Acquisition, AcquisitionResponseDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<CreateAcquisitionCommand, Acquisition>();
            CreateMap<UpdateAcquisitionCommand, Acquisition>();
            CreateMap<GetAllAcquisitionQuery, GetAllAcquisitionParameter>();

            //AcquisitionItem
            CreateMap<AcquisitionItem, AcquisitionItemDto>()
                .ForMember(dest => dest.Inventory, opt => opt.MapFrom(src => src.Inventory));

            //Order
            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.MaterialsUsed, opt => opt.MapFrom(src => src.MaterialsUsed));
            CreateMap<CreateOrderCommand, Order>();
            CreateMap<UpdateOrderCommand, Order>();
            CreateMap<GetAllOrderQuery, GetAllOrderParameter>();

            //MaterialUsed
            CreateMap<MaterialUsed, MaterialUsedDto>()
                .ForMember(dest => dest.Inventory, opt => opt.MapFrom(src => src.Inventory));
        }
    }
}
