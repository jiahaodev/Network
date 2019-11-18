using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTank : BaseTank
{
    //Ԥ����Ϣ���ĸ�ʱ�䵽���ĸ�λ��
    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 forecastPos;
    private Vector3 forecastRot;
    private float lastTurretY;
    private float forecastTurretY;
    private float lastGunX;
    private float forecastGunX;
    private float forecastTime;

    //��ʼ��
    public override void Init(string skinPath)
    {
        base.Init(skinPath);
        //���������˶�Ӱ��
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Rigidbody>().useGravity = false;
        //��ʼ��Ԥ����Ϣ
        lastPos = transform.position;
        lastRot = transform.eulerAngles;
        forecastPos = transform.position;
        forecastRot = transform.eulerAngles;
        lastTurretY = turret.localEulerAngles.y; 
        forecastTurretY = turret.localEulerAngles.y;
        lastGunX = gun.localEulerAngles.x;
        forecastGunX = gun.localEulerAngles.x;
        forecastTime = Time.time;
    }

    new void Update() {
        base.Update();
        //����λ��
        ForecastUpdate();
    }


    //�ƶ�ͬ��
    public void SyncPos(MsgSyncTank msg) {
        //Ԥ��λ��
        Vector3 pos = new Vector3(msg.x, msg.y, msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        //forecastPos = pos + 2*(pos - lastPos);
        //forecastRot = rot + 2*(rot - lastRot);
        forecastPos = pos;  //���治Ԥ��
        forecastRot = rot;
        forecastTurretY = msg.turretY;
        forecastGunX = msg.gunX;
        //����
        lastPos = pos;
        lastRot = rot;
        lastTurretY = turret.localEulerAngles.y;
        lastGunX = turret.localEulerAngles.x;
        forecastTime = Time.time;
    }


    //����λ��  //todo ??
    public void ForecastUpdate() {
        //ʱ��
        float t = (Time.time - forecastTime) / CtrlTank.syncInterval;
        t = Mathf.Clamp(t, 0f, 1f);
        //λ��
        Vector3 pos = transform.position;
        pos = Vector3.Lerp(pos, forecastPos, t);
        transform.position = pos;
        //��ת
        Quaternion quat = transform.rotation;
        Quaternion forcastQuat = Quaternion.Euler(forecastRot);
        quat = Quaternion.Lerp(quat, forcastQuat, t);
        transform.rotation = quat;
        //������ת���Ĵ�����
        float axis = transform.InverseTransformPoint(forecastPos).z;
        axis = Mathf.Clamp(axis * 1024, -1f, 1f);
        WheelUpdate(axis);
        //�ڹ�
        Vector3 le = turret.localEulerAngles;
        le.y = Mathf.LerpAngle(le.y, forecastTurretY, t);
        turret.localEulerAngles = le;
        //����
        le = gun.localEulerAngles;
        le.x = Mathf.LerpAngle(le.x, forecastGunX, t);
        gun.localEulerAngles = le;
    }

    //����
    public void SyncFire(MsgFire msg) {
        Bullet bullet = Fire();
        //��������
        Vector3 pos = new Vector3(msg.x,msg.y,msg.z);
        Vector3 rot = new Vector3(msg.ex, msg.ey, msg.ez);
        bullet.transform.position = pos;
        bullet.transform.eulerAngles = rot;
    }


}