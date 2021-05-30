using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieuKhienGiaoDien : MonoBehaviour
{
    /// DIEU KHIEN CAC GIAO DIEN NGUOI DUNG
    public Text thongBaoSoLuongNuocDi;
    public Text thongBaonguoiDung;
                                      
    private int soLuongNuocDi = 0;
    public void tangSoLuongNuocDi() {
        soLuongNuocDi++;
        thongBaoSoLuongNuocDi.text ="So nuoc di duoc: "+ soLuongNuocDi;
    }

    public void resetSoLuongNuocDi()
    {
        soLuongNuocDi++;
        thongBaoSoLuongNuocDi.text = "So nuoc di duoc: 0" ;
    }

    public void tatThongBaoNguoiDung() {
        thongBaonguoiDung.text = "";
    }

    public void thongBaoEndGame() { 
        thongBaonguoiDung.text = "Ban da hoan thanh game. Nhan A de Reset";
    }

    public void thongBaoKhongLayDia()
    {
       
        thongBaonguoiDung.text = "Ban phai chon vao cot co dia";
    }
    public void thongBaoThaDiaKhongHopLe()
    {

        thongBaonguoiDung.text = "Moi ban chon cot khac";
    }
}
