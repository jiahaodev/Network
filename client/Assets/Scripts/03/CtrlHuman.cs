/****************************************************
    文件：CtrlHuman.cs
	作者：JiahaoWu
    邮箱: jiahaodev@163.com
    日期：2019/11/10 18:32:28
	功能：CtrlHuman。 用户控制主角移动
*****************************************************/

using UnityEngine;

public class CtrlHuman : BaseHuman 
{
    new void Start() {
        base.Start();
    }

    new void Update() {
        base.Update();
        CtrlHumanMove();

    }

    private void CtrlHumanMove() {
        //移动
        if (Input.GetMouseButtonDown(0))  //鼠标左键
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray,out hit);
            if (hit.collider.tag == "Terrain")
            {
                MoveTo(hit.point);
                //发送协议
                string sendStr = "Move|";
                sendStr += SimpleNetManager.GetDesc() + ",";
                sendStr += hit.point.x + ",";
                sendStr += hit.point.y + ",";
                sendStr += hit.point.z + ",";
                SimpleNetManager.Send(sendStr);
            }
        }
        //攻击
        if (Input.GetMouseButtonDown(1))  //鼠标右键
        {
            if (isAttacking) return;
            if (isMoving) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray,out hit);

            transform.LookAt(hit.point);
            Attack();

            //发送协议(攻击)
            string sendStr = "Attack|";
            sendStr += SimpleNetManager.GetDesc() + ",";
            sendStr += transform.eulerAngles.y + ",";
            SimpleNetManager.Send(sendStr);

            //攻击判定
            Vector3 lineEnd = transform.position + 0.5f * Vector3.up;
            Vector3 lineStart = lineEnd + 20f * transform.forward;
            if (Physics.Linecast(lineStart,lineEnd,out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj == gameObject)
                    return;
                SyncHuman h = hitObj.GetComponent<SyncHuman>();
                if (h == null)
                    return;
                sendStr = "Hit|";
                sendStr += SimpleNetManager.GetDesc() + ",";
                sendStr += h.desc + ",";
                SimpleNetManager.Send(sendStr);
            }
        }
    }
}