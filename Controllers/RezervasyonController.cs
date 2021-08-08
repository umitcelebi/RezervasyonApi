using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ReservationApp.Models;

namespace ReservationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RezervasyonController : ControllerBase
    {
        [HttpPost]
        public IActionResult RezervasyonPostAsync(Rezervasyon rezervasyon)
        {
            List<YerlesimAyrinti> yerlesimAyrintiList = new List<YerlesimAyrinti>();
            YerlesimAyrinti yerlesimAyrinti = null;
            Response response;
            //Farklı vagonlara rezervasyon yapılıp yapılmadığını kontrol ediyoruz.
            if (!rezervasyon.KisilerFarkliVagonlaraYerlestirilebilir)
            {
                
                foreach (var vagon in rezervasyon.Tren.Vagonlar)
                {
                    /*Vagonun dolu koltuk oranının kapasitenin yüzde 70inin altında mı ve
                    rezervasyon yapılacak kişi sayısını ekleyince kapasiyeyi geçiyor mu onu kontrol ediyoruz.*/
                    if((vagon.Kapasite*70/100)>=vagon.DoluKoltukAdet && vagon.DoluKoltukAdet+rezervasyon.RezervasyonYapilacakKisiSayisi<=vagon.Kapasite)
                    {
                        yerlesimAyrinti = new()
                        {
                            KisiSayisi = rezervasyon.RezervasyonYapilacakKisiSayisi,
                            VagonAdi = vagon.Ad
                        };
                        yerlesimAyrintiList.Add(yerlesimAyrinti);
                    }
                }
                
                if (!(yerlesimAyrintiList.Count>0))
                {
                    response = new()
                    {
                        RezervasyonYapilabilir = false,
                        YerlesimAyrinti = new List<YerlesimAyrinti>()
                    };
                    return Ok(response);
                }
                response = new()
                {
                    YerlesimAyrinti = yerlesimAyrintiList,
                    RezervasyonYapilabilir = true
                };
                return Ok(response);
            }
            // Faklı vagonlara rezervasyon yapılıyorsa;
            else
            {
                foreach (var vagon in rezervasyon.Tren.Vagonlar)
                {
                    if (vagon.Kapasite * 70 / 100 >= vagon.DoluKoltukAdet)
                    {
                        int bosKoltuk = vagon.Kapasite - vagon.DoluKoltukAdet;
                        
                        if (bosKoltuk >= rezervasyon.RezervasyonYapilacakKisiSayisi)
                        {
                            yerlesimAyrinti = new()
                            {
                                KisiSayisi = rezervasyon.RezervasyonYapilacakKisiSayisi,
                                VagonAdi = vagon.Ad
                            };
                            yerlesimAyrintiList.Add(yerlesimAyrinti);
                            response = new()
                            {
                                YerlesimAyrinti = yerlesimAyrintiList,
                                RezervasyonYapilabilir = true
                            };
                            return Ok(response);
                        }
                        else
                        {
                            yerlesimAyrinti = new()
                            {
                                KisiSayisi = vagon.Kapasite - vagon.DoluKoltukAdet,
                                VagonAdi = vagon.Ad
                            };
                            yerlesimAyrintiList.Add(yerlesimAyrinti);
                            rezervasyon.RezervasyonYapilacakKisiSayisi -= yerlesimAyrinti.KisiSayisi;
                        }
                    }
                }
            }

            response = new()
            {
                RezervasyonYapilabilir = false,
                YerlesimAyrinti = yerlesimAyrintiList
            };
            return Ok(response);
            
        }
    }
}
