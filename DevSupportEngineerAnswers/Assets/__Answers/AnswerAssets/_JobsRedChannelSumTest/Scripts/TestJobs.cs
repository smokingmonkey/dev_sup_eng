// This script use the Job System to calculate the sum of the R channel of a texture
// We will split the texture into four regions of equal size in order to compute every
// fourth part in jobs running in parallel

using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class TestJobs : MonoBehaviour
{

    public string textureName;
    public Texture2D sourceTex;
    Color[] pix;
    int pixLenghtquartem;//Here we will store the fourth part of our texture total lenght

    // Native arrays for quartem R channel sum
    NativeArray<float> rChannelNative1;
    NativeArray<float> rChannelNative2;
    NativeArray<float> rChannelNative3;
    NativeArray<float> rChannelNative4;

    // Native arrays for output data 
    NativeArray<float> quartemSum1;
    NativeArray<float> quartemSum2;
    NativeArray<float> quartemSum3;
    NativeArray<float> quartemSum4;

    // Start is called before the first frame update
    void Start()
    {        
        pix = sourceTex.GetPixels();//Get the texture pixel data

        // We will force "pix" to be divisible by 4 to avoid handling non divisible
        // by 4 textures in the sake of the premises of the exercise
        if (pix.Length % 4 != 0)
        {
            return;
        }
        pixLenghtquartem = pix.Length / 4;      

    }

    private void Update()
    {
        // Our arrays are initialized here 
        //When scheduling a job we need to use Allocator.TempJob
        rChannelNative1 = new NativeArray<float>(pixLenghtquartem, Allocator.TempJob);
        rChannelNative2 = new NativeArray<float>(pixLenghtquartem, Allocator.TempJob);
        rChannelNative3 = new NativeArray<float>(pixLenghtquartem, Allocator.TempJob);
        rChannelNative4 = new NativeArray<float>(pixLenghtquartem, Allocator.TempJob);

        quartemSum1 = new NativeArray<float>(1, Allocator.TempJob);
        quartemSum2 = new NativeArray<float>(1, Allocator.TempJob);
        quartemSum3 = new NativeArray<float>(1, Allocator.TempJob);
        quartemSum4 = new NativeArray<float>(1, Allocator.TempJob);

        //We split our texture data into four parts
        for (int i = 0; i < pixLenghtquartem; i++)
        {
            rChannelNative1[i] = pix[i].r;
            rChannelNative2[i] = pix[i + 1 * pixLenghtquartem].r;
            rChannelNative3[i] = pix[i + 2 * pixLenghtquartem].r;
            rChannelNative4[i] = pix[i + 3 * pixLenghtquartem].r;
        }

        //We store our jobs here to be scheduled
        NativeList<JobHandle> jobHandlesList = new NativeList<JobHandle>(Allocator.TempJob);

        SumRedChannel job = new SumRedChannel
        {            
            rPix = rChannelNative1,
            quartemSum = quartemSum1
        };
        jobHandlesList.Add(job.Schedule());

        SumRedChannel job2 = new SumRedChannel
        {
            rPix = rChannelNative2,
            quartemSum = quartemSum2
        };
        jobHandlesList.Add(job2.Schedule());

        SumRedChannel job3 = new SumRedChannel
        {
            rPix = rChannelNative3,
            quartemSum = quartemSum3
        };
        jobHandlesList.Add(job3.Schedule());

        SumRedChannel job4 = new SumRedChannel
        {
            rPix = rChannelNative4,
            quartemSum = quartemSum4
        };
        jobHandlesList.Add(job4.Schedule());     

        //Tell the JobHandle to complete the jobs
        JobHandle.CompleteAll(jobHandlesList);

        float sum = quartemSum1[0] + quartemSum2[0] + quartemSum3[0] + quartemSum4[0];
        Debug.Log("The sum of the R channel of " + textureName + " texture is: " + sum);
       
        //Finally we have to dispose the allocated memory before returning in our Update function
        jobHandlesList.Dispose();        
       
        rChannelNative1.Dispose();
        rChannelNative2.Dispose();
        rChannelNative3.Dispose();
        rChannelNative4.Dispose();

        quartemSum1.Dispose();
        quartemSum2.Dispose();
        quartemSum3.Dispose();
        quartemSum4.Dispose();

       
    }
    
    //Our Job declaration

    //BurstCompatible will speed up our jobs
    [BurstCompatible]
    struct SumRedChannel : IJob
    {                   
        public NativeArray<float> rPix;//the red component of our texture
        public NativeArray<float> quartemSum;//the data of the size of the fourth part
                                             //of our texture lenght         
        
        //The Job system will execute this
        public void Execute()
        {            
            for (int i = 0; i < rPix.Length; i++)
            {
                quartemSum[0] += rPix[i];
            }
        }
    }
}
