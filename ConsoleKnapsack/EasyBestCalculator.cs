using GAMultidimKnapsack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleKnapsack
{
    class EasyBestCalculator
    {
        long possibleConfigsAmount;
        int itemsAmount,dimensions;
        double[] restrictions, itemsCosts;
        double[,] itemsSet;
        public EasyBestCalculator(int itemsAm, int dim, double[] rest, double[] costs, double[,] myItemsSet)
        {
            itemsAmount = itemsAm;
            restrictions = rest;
            dimensions = dim;
           // itemsSet = new double[itemsAm, dim];
            itemsSet = myItemsSet;
            itemsCosts = costs;
            //possibleConfigsAmount = Convert.ToInt64(Math.Pow(2, itemsAmount));
        }
        //double GetBestValue()
        //{
        //    double maxValue = 0;
        //    KnapsackConfig k = new KnapsackConfig(itemsAmount);
        //    for(long i=0;i< possibleConfigsAmount; i++)
        //    {
        //        k = GetKnapsackByNumber(i);
        //        if (IsValid(k) && GetKnapsackCost(k) > maxValue)
        //            maxValue = GetKnapsackCost(k);
        //    }
        //}
        KnapsackConfig GetKnapsackByNumber(long number )
        {
            var currentValue = number;
            int i = 0;
            KnapsackConfig resultKnapsack = new KnapsackConfig(itemsAmount);
            while(currentValue!=0)
            {
                if (currentValue % 2 == 0)
                    resultKnapsack.setValueToActive(i);
                currentValue = currentValue / 2;
                i++;
            }
            return resultKnapsack;
        }

        private bool IsValid(KnapsackConfig config)
        {
            double[] summ = new double[dimensions];
            for (var i = 0; i < itemsAmount; i++)
            {
                if (config.isValueActive(i))
                {
                    for (var j = 0; j < dimensions; j++)
                    {
                        summ[j] += itemsSet[i, j];
                        if (summ[j] > restrictions[j]) return false;
                    }
                }
                //Amount of items is much bigger than number of dimensions, so we can do checks on every turn. 
            }
            return true;
        }

        private double GetKnapsackCost(KnapsackConfig sack)
        {
            double count = 0;
            for (int i = 0; i < itemsAmount; i++)
                if (sack.isValueActive(i))
                    count += itemsCosts[i];

            return count;
        }
    }
}
