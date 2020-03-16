﻿using AbstractRemontBusinessLogic.BindingModels;
using AbstractRemontBusinessLogic.Enums;
using AbstractRemontBusinessLogic.HelperModels;
using AbstractRemontBusinessLogic.Interfaces;
using AbstractRemontBusinessLogic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractRemontBusinessLogic.BusinessLogics
{
    // Создание заказа и смена его статусов
    public class MainLogic
    {
        //private readonly IComponentLogic componentLogic;
        //private readonly IShipLogic shipLogic;
        
        private readonly IRemontLogic orderLogic;
        public MainLogic(IRemontLogic orderLogic)
        {
            this.orderLogic = orderLogic;
        }
        /*
        public ReportLogic(IShipLogic shipLogic, IComponentLogic componentLogic, IRemontLogic orderLogic)
        {
            this.shipLogic = shipLogic;
            this.componentLogic = componentLogic;
            this.orderLogic = orderLogic;
        }
        /// Получение списка компонент с указанием, в каких изделиях используются
        /// </summary>
        /// <returns></returns>
        public List<ReportShipComponentViewModel> GetProductComponent()
        {
            var components = componentLogic.Read(null);
            var products = shipLogic.Read(null);
            var list = new List<ReportShipComponentViewModel>();
            foreach (var component in components)
            {
                var record = new ReportShipComponentViewModel
                {
                    ComponentName = component.ComponentName,
                    Ships = new List<Tuple<string, int>>(),
                    TotalCount = 0
                };
                foreach (var product in products)
                {
                    if (product.ShipComponents.ContainsKey(component.Id))
                    {
                        record.Ships.Add(new Tuple<string, int>(product.ShipName, product.ShipComponents[component.Id].Item2));
                        record.TotalCount += product.ShipComponents[component.Id].Item2;
                    }
                }
                list.Add(record);
            }
            return list;
        }
        /// <summary>
        /// Получение списка заказов за определенный период
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<ReportRemontsViewModel> GetOrders(ReportBindingModel model)
        {
            return orderLogic.Read(new RemontBindingModel { DateFrom = model.DateFrom, DateTo = model.DateTo })
            .Select(x => new ReportRemontsViewModel
            {
                DateCreate = x.DateCreate,
                ShipName = x.ShipName,
                Count = x.Count,
                Sum = x.Sum,
                Status = x.Status
            })
            .ToList();
        }
        /// <summary>
        /// Сохранение компонент в файл-Word
        /// </summary>
        /// <param name="model"></param>
        public void SaveComponentsToWordFile(ReportBindingModel model)
        {
            SaveToWord.CreateDoc(new WordInfo
            {
                FileName = model.FileName,
                Title = "Список компонент",
                Components = componentLogic.Read(null)
            });
        }
        /// <summary>
        /// Сохранение компонент с указаеним продуктов в файл-Excel
        /// </summary>
        /// <param name="model"></param>
        public void SaveProductComponentToExcelFile(ReportBindingModel model)
        {
            SaveToExcel.CreateDoc(new ExcelInfo
            {
                FileName = model.FileName,
                Title = "Список компонент",
                ProductComponents = GetProductComponent()
            });
        }
        /// <summary>
        /// Сохранение заказов в файл-Pdf
        /// </summary>
        /// <param name="model"></param>
        public void SaveOrdersToPdfFile(ReportBindingModel model)
        {
            SaveToPdf.CreateDoc(new PdfInfo
            {
                FileName = model.FileName,
                Title = "Список заказов",
                DateFrom = model.DateFrom.Value,
                DateTo = model.DateTo.Value,
                Orders = GetOrders(model)
            });
        }
        */
        public void CreateRemont(CreateRemontBindingModel model)
        {
            orderLogic.CreateOrUpdate(new RemontBindingModel
            {
                ShipId = model.ShipId,
                Count = model.Count,
                Sum = model.Sum,
                DateCreate = DateTime.Now,
                Status = RemontStatus.Принят
            });
        }

        public void TakeRemontInWork(ChangeStatusBindingModel model)
        {
            var order = orderLogic.Read(new RemontBindingModel { Id = model.RemontId })?[0];
            if (order == null)
            {
                throw new Exception("Не найден заказ");
            }
            Console.WriteLine("STATUS " + order.Status);
            if (order.Status != RemontStatus.Принят)
            {
                throw new Exception("Заказ не в статусе \"Принят\"");
            }
            orderLogic.CreateOrUpdate(new RemontBindingModel
            {
                Id = order.Id,
                ShipId = order.ShipId,
                Count = order.Count,
                Sum = order.Sum,
                DateCreate = order.DateCreate,
                DateImplement = DateTime.Now,
                Status = RemontStatus.Выполняется
            });
        }

        public void FinishRemont(ChangeStatusBindingModel model)
        {
            var order = orderLogic.Read(new RemontBindingModel { Id = model.RemontId})?[0];
            if (order == null)
                throw new Exception("Не найден заказ");
            if (order.Status != RemontStatus.Выполняется)
                throw new Exception("Заказ не в статусе \"Выполняется\"");
            orderLogic.CreateOrUpdate(new RemontBindingModel
            {
                Id = order.Id,
                ShipId = order.ShipId,
                Count = order.Count,
                Sum = order.Sum,
                DateCreate = order.DateCreate,
                DateImplement = order.DateImplement,
                Status = RemontStatus.Готов
            });
        }

        public void PayRemont(ChangeStatusBindingModel model)
        {
            var order = orderLogic.Read(new RemontBindingModel { Id = model.RemontId})?[0];
            if (order == null)
                throw new Exception("Не найден заказ");
            if (order.Status != RemontStatus.Готов)
                throw new Exception("Заказ не в статусе \"Готов\"");
            orderLogic.CreateOrUpdate(new RemontBindingModel
            {
                Id = order.Id,
                ShipId = order.ShipId,
                Count = order.Count,
                Sum = order.Sum,
                DateCreate = order.DateCreate,
                DateImplement = order.DateImplement,
                Status = RemontStatus.Оплачен
            });
        }
    }
}
