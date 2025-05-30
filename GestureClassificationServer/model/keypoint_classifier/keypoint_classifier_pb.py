#!/usr/bin/env python
# -*- coding: utf-8 -*-
import numpy as np
import tensorflow as tf


class KeyPointClassifier_pb(object):
    def __init__(
        self,
        model_path='model/keypoint_classifier/classification.pb12300'
    ):
        self.model = tf.saved_model.load(model_path)
        self.infer = self.model.signatures["serving_default"]
        #print(self.model.signatures["serving_default"])
        
    def __call__(
        self,
        landmark_list
    ):
        
        inputs=tf.convert_to_tensor([landmark_list], dtype=tf.float32)

        output = self.infer(inputs=inputs)

        result = output['output_0'].numpy()
        result_index = np.argmax(np.squeeze(result))

        return result_index
