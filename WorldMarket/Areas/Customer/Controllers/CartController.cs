﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using WorldMarket.DataAccess.Repository.IRepository;
using WorldMarket.Models;
using WorldMarket.Models.View_Models;

namespace WorldMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            var claim = identityClaims.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new CartVM()
            {
                ListCart = _unitOfWork.ShoppingCarts.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };
            foreach(var item in ShoppingCartVM.ListCart)
            {
                item.Price = GetPriceByQuantity(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                ShoppingCartVM.TotalPrice += (item.Price * item.Count);
            }
            
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCarts.GetFirstOrDefault(u => u.Id == cartId);

            _unitOfWork.ShoppingCarts.Incrementation(cartFromDb, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }



        private double GetPriceByQuantity(double quantity, double price, double price50, double price100) {
        
                if(quantity <= 50)
                {
                 return price;
            }
            else
            {
                if(quantity <= 100)
                {
                    return price50;
                }
                return price100;
            }
        }
    }
}
