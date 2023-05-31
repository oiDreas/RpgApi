using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    public class DisputasController : Controller
    {
        //public string uriBaseold = "http://localhost:5164/Disputas/";   
        public string uriBase = "http://luizsouza.somee.com/RpgApi/";
        //xyz tem que ser substituído pelo nome do seu site na API.

        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = uriBase + "Personagens/GetAll";
                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                    ViewBag.ListaAtacantes = listaPersonagens;
                    ViewBag.ListaOponentes = listaPersonagens;

                    return View();
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> IndexAsync(DisputaViewModel disputa)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Disputas/Arma";

                var content = new StringContent(JsonConvert.SerializeObject(disputa));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    disputa = await Task.Run(() => JsonConvert.DeserializeObject<DisputaViewModel>(serialized));
                    TempData["Mensagem"] = disputa.Narracao;
                    return RedirectToAction("Index", "Personagens");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> IndexHabilidadesAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = uriBase + "Personagens/GetAll";
                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                    ViewBag.ListaAtacantes = listaPersonagens;
                    ViewBag.ListaOponentes = listaPersonagens;
                    //return View(); //Remova esta linha
                }
                else
                    throw new System.Exception(serialized);

                string uriBuscaHabilidades = uriBase + "PersonagemHabilidades/GetHabilidades";


                response = await httpClient.GetAsync(uriBuscaHabilidades);
                serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<HabilidadeViewModel> listaHabilidades = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<HabilidadeViewModel>>(serialized));
                    ViewBag.ListaHabilidades = listaHabilidades;
                }
                else
                    throw new System.Exception(serialized);

                return View("IndexHabilidades");
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> IndexHabilidadesAsync(DisputaViewModel disputa)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplementar = "Disputas/Habilidade";
                var content = new StringContent(JsonConvert.SerializeObject(disputa));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    disputa = await Task.Run(() =>
                    JsonConvert.DeserializeObject<DisputaViewModel>(serialized));
                    TempData["Mensagem"] = disputa.Narracao;
                    return RedirectToAction("Index", "Personagens");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DisputaGeralAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = uriBase + "Personagens/GetAll";

                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);

                string serialized = await response.Content.ReadAsStringAsync();

                List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                string uriDisputa = uriBase + "Disputas/DisputaEmGrupo";

                DisputaViewModel disputa = new DisputaViewModel();
                disputa.ListaIdPersonagens = new List<int>();
                disputa.ListaIdPersonagens.AddRange(listaPersonagens.Select(p => p.Id));//using System.Linq

                var content = new StringContent(JsonConvert.SerializeObject(disputa));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await httpClient.PostAsync(uriDisputa, content);

                serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    disputa = await Task.Run(() => JsonConvert.DeserializeObject<DisputaViewModel>(serialized));
                    TempData["Mensagem"] = string.Join("<br>", disputa.Resultados);
                }
                else
                    throw new System.Exception(serialized);

                return RedirectToAction("Index", "Personagens");
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index", "Personagens");
            }
        }

        [HttpGet]
        public async Task<ActionResult> IndexDisputasAsync()
        {
            try
            {
                string uriComplementar = "Listar";
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<DisputaViewModel> lista = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<DisputaViewModel>>(serialized));
                    return View(lista);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }



        [HttpGet]
        public async Task<ActionResult> ApagarDisputasAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string uriComplementar = "ApagarDisputas";
                HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "Disputas apagadas com sucessso.";
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("IndexDisputas", "Disputas");
        }




    }
}