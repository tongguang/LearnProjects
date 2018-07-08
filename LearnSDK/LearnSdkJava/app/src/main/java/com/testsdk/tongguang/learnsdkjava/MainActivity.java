package com.testsdk.tongguang.learnsdkjava;

import android.app.AlertDialog;
import android.content.DialogInterface;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {
    public void TestFun1() {
        AlertDialog dialog = new AlertDialog.Builder(this)
                .setTitle("普通对话框")
                .setNegativeButton("取消", null)
                .setPositiveButton("确定", new DialogInterface.OnClickListener() {

                    public void onClick(DialogInterface dialog, int which) {
                        UnityPlayer.UnitySendMessage("GameRoot", "OnJavaMessage", "test_ok");
                    }
                })
                .setMessage("确认删除？").create();
        dialog.show();
    }
}
