using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    private const int SO_LUONG_COT = 3;
    private const int SO_LUONG_DIA = 3;
    /// Mảng đại diện cho các đĩa trong game (với kiểu là gameobject)
    /// Cac đĩa phải được đặt thứ tự theo kích thước tăng dần, 
    /// VD: đĩa thứ 0 là đĩa nhỏ nhất, đĩa thứ 2 là đĩa lớn nhất 
    public GameObject[] mangDia = new GameObject[SO_LUONG_DIA];


    /// Vị trí cột mục tiêu mà ta muốn di chuyển tới
    public int viTriDat = 2;

    /// Biến đại diện mũi tên
    public GameObject muiTen;

    /// Quy định vị trí on top để di chuyển đĩa công thức : = vị trí cột.y + 2(đơn vị)  
    public Transform[] viTriTop = new Transform[SO_LUONG_COT];


    /// Mảng chứa các ngăn xếp(gọi là stack)
    /// bản chát các cột là các ngăn xếp đĩa, đĩa trên cùng là đĩa sẽ được sử dụng 
    /// Các stack này sẽ chứa 1 số (kiểu này gọi là number hoặc integer), các số này là chỉ số tham chiếu đến các
    /// đối tượng đĩa ở mảng bên trên
    private Stack<int>[] mangCot = new Stack<int>[SO_LUONG_COT];

    
    /// Đại diện cho đĩa được chọn, -1 nghiã là không có đĩa nào được chọn
    private int diaDuocChon = -1;

    /// Đại diện cho ngẵn xếp (cột) đang được chọn
    private int cotDuocChon = 0;

    /// Kịch bản hoạt động của đoạn script này như sau:
    /// B1. người dùng tiến hành di chuyển để lựa chọn đĩa sẽ di chuyển bằng bàn phím
    /// Nếu cột được chọn ko có đĩa nào thì ta sẽ không thực hiện việc gì cả
    /// B2. Lấy đĩa trên cùng của ngăn xếp(cột) mà ta đã chọn. Bằng hàm pop().
    /// B3. Người dùng tiếp tục di chuyển các phím điều hướng để đặt đĩa đến vị trí ngăn xếp khác
    /// Lưu ý: - Lúc này cần phải hiển thị giao diện sao cho người dùng biết mình đang chọn ngăn xếp (côt nào), tuy nhiên
    ///          thiết kế như nào phải tùy theo ý của người thế kế và cả môi trường 2d hoặc 3d
    ///        - Phải thiết kế thêm chức năng hủy việc duy chuyển
    /// B4. Ngăn xếp (cột) được di chuyển phải thỏa điều kiện đó là đĩa trên cùng của ngăn xếp phải có kích thước 
    /// lớn hơn kích thước của đĩa được di chuyển
    /// CUỐI CÙNG: BÀI toán dừng lại khi ngăn xếp ở vị trí mong muốn (được quy định ở biến viTriDat) có đầy đủ 3 đĩa


    private bool endGame = false;
    private DieuKhienGiaoDien ui;
    private void Awake()
    {
        ui = GetComponent<DieuKhienGiaoDien>();
    }

    private void Start()
    {
        loadGame();
    }

    private void Update()
    {
        ///KIEM TRA CO RESET GAME KHONG
        if (Input.GetKeyDown(KeyCode.A))
        {
            loadGame();
        }

        if (endGame) return;
        //// Tiến hành xem người dùng ấn các phím mũi tên trái hoặc phải
        DieuHuongMuiTen();
        //// Tiến hành lắng nghe sự kiện người dùng tác động đến đĩa (rút ra hoặc thả xuống) bằng phím space
        TacDongLenDia();
       
    }

    private void TacDongLenDia()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            /// LẤY ĐĨA : Nếu không có đĩa nào được chọn thì khi ta nhấn space đó là lúc người dùng chọn đĩa
            if (diaDuocChon == -1)
            {
                /// Lấy đĩa trên cùng của cột được chọn bằng hàm pop
                /// Lứu ý chỉ lấy được khi ngăn xếp không được rỗng

                if (mangCot[cotDuocChon].Count == 0) {
                    ui.thongBaoKhongLayDia();
                    return;
                }

                diaDuocChon = mangCot[cotDuocChon].Pop();
                layDia();
            }
            /// THẢ ĐĨA : Nếu ta đang giữ đĩa thì lúc người dùng nhấn space là lúc người dùng thả đĩa
            else
            {
                /// kiểm tra cột có hợp lệ không, tức là đĩa ở dưới phải lớn hơn đĩa ở trên
           
                if (mangCot[cotDuocChon].Count == 0 || diaDuocChon < mangCot[cotDuocChon].Peek())
                {
                    thaDia();
                    ui.tangSoLuongNuocDi();
                    mangCot[cotDuocChon].Push(diaDuocChon);
                    diaDuocChon = -1;
                }
                else

                {
                    ui.thongBaoThaDiaKhongHopLe();
                }


                /// THẢ ĐĨA XONG THÌ KIỂM TRA BÀI TOÁN KẾT THÚC CHƯA
                if (ketThucGame())
                {
                    ui.thongBaoEndGame();
                    endGame = true;
                }
            }
        }
    }

    private void DieuHuongMuiTen()
    {
        bool diChuyen = false;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cotDuocChon = cotDuocChon > 0 ? cotDuocChon - 1 : SO_LUONG_COT - 1;
            diChuyen = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            cotDuocChon = cotDuocChon < SO_LUONG_COT - 1 ? cotDuocChon + 1 : 0;
            diChuyen = true;
        }

        if (diChuyen) {
            ui.tatThongBaoNguoiDung();
            /// DI CHUYỂN BIỂU TƯỢNG MŨI TÊN
            muiTen.transform.position = new Vector2(viTriTop[cotDuocChon].position.x, viTriTop[cotDuocChon].position.y + 2f);

            /// NẾU CÓ ĐĨA ĐƯỢC CHỌN THÌ CHO NÓ DI CHUYỂN THEO LUÔN
            if (diaDuocChon != -1)
                mangDia[diaDuocChon].transform.position = viTriTop[cotDuocChon].position;

        }

    }


    ////=========== CÁC HÀM HỖ TRỢ ================
    private bool ketThucGame()
    {
    
    
        return mangCot[viTriDat].Count == SO_LUONG_COT;
    }

    /// Hàm này hỗ trơ việc biểu diễn việc lấy đĩa cho người dùng xem
    private void layDia() {
        if (diaDuocChon == -1) return;
        mangDia[diaDuocChon].GetComponent<Rigidbody2D>().mass = 0;
        mangDia[diaDuocChon].GetComponent<Rigidbody2D>().gravityScale = 0;
        mangDia[diaDuocChon].transform.position = viTriTop[cotDuocChon].position;
    }

    /// Hàm này biểu diễn cho việc thả đĩa ra
    private void thaDia() {
        if (diaDuocChon == -1) return;
        mangDia[diaDuocChon].GetComponent<Rigidbody2D>().mass = 1;
        mangDia[diaDuocChon].GetComponent<Rigidbody2D>().gravityScale = 1;
    }


    void loadGame()
    {
        mangCot = new Stack<int>[SO_LUONG_COT];
        for (int i = 0; i < SO_LUONG_COT; i++)
            mangCot[i] = new Stack<int>();
        mangCot[0].Push(2);
        mangCot[0].Push(1);
        mangCot[0].Push(0);
        cotDuocChon = 0;
        diaDuocChon = -1;

        muiTen.transform.position = new Vector2(viTriTop[cotDuocChon].position.x, viTriTop[cotDuocChon].position.y + 2f);
        mangDia[2].transform.position = new Vector2(viTriTop[cotDuocChon].position.x, viTriTop[cotDuocChon].position.y - 1f);
        mangDia[1].transform.position = new Vector2(viTriTop[cotDuocChon].position.x, viTriTop[cotDuocChon].position.y + 0f);
        mangDia[0].transform.position = new Vector2(viTriTop[cotDuocChon].position.x, viTriTop[cotDuocChon].position.y + 1f);
        ui.tatThongBaoNguoiDung();
        ui.resetSoLuongNuocDi();
        endGame = false;
    }                                             
}
