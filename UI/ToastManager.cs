using EasyUI.Toast ;using UnityEngine;


public class ToastManager: MonoBehaviour{
    public void MakeAToast(string texts){
        Toast.Show(texts);
    }
}