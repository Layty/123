using System;

namespace �����ǻ���Դ���ص������.Model
{
    /// <inheritdoc />
    /// <summary>
    /// ��ʾ���������¼����࣬���ڴ��ݴ����¼�����ز���
    /// </summary>
    public class MySerialEventArgs : EventArgs
    {
        public byte[] DataBytes; //���ڵķ��ͻ�������ݣ�
        public Exception Ex; //��������쳣��Ϣ
        public int DelayTime; //���ڳ��������ȴ�������Ӧ��ʱ��
        public bool OverTimeMonitor; //���������Ƿ�Ϊ������ȡָ����������ȡ���ע��ʱ�¼�
        public string ResponseTime; //�����󵽽��շ�������֮���ʱ���������պ󵽷�����Ӧ����֮���ʱ����
        public int ChannelNum;
        public string ConfigMessage;
        public bool IsStringData;
    }
}