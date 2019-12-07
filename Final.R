zip.train <- as.data.frame(read.table(file="~/Desktop/Data/zip.train",header=FALSE))
zip.test <- as.data.frame(read.table(file="~/Desktop/Data/zip.test",header=FALSE))

train_label <- zip.train[,1]
test_label <- zip.test[,1]
#Create dataset for training
train3 <- zip.train[train_label == 3 , -1]
train5 <- zip.train[train_label == 5, -1]
#display image
zimage <- function(vec){
  img <-matrix(vec[1:256],nrow=16, ncol=16)
  img<-t(apply(-img,1,rev))
  image(img,col=grey(seq(0,1,length=256)))
}

# problem 2: step 1
computeTraining <- function(trainingDS, class1, class2){
	#return #sample, mu, covariance
	label <- trainingDS[,1]
	train1 <- trainingDS[label == class1, -1]
	train2 <- trainingDS[label == class2, -1]
	#trainc <- trainingDS[label == class2 | label ==  class1, -1]
	n1=dim(train1)[1]
	n2=dim(train2)[1]
	nc=n1+n2
	#covariance
	ct1 <- cov(train1)
	ct2 <- cov(train2)
	ctc <- 1/(nc-2)*((n1-1)*ct1+(n2-1)*ct2)
	#mean
	mt1 <- colMeans(train1)
	mt2 <- colMeans(train2)
	mtc <- 1/(nc)*((n1)*ct1+(n2)*ct2)
	return (list("n1"=n1,"n2"=n2,"nc"=nc,"mt1"=mt1, "mt2"=mt2, "mtc"=mtc,"ct1"=ct1, "ct2"=ct2, "ctc"=ctc))
}
a <- computeTraining(zip.train, 3, 5)

#Problem 2 step 2 for LDA
LDA <- function(trainingDS, class1, class2){
  #return classifier that return >0 if data in class1
  label <- trainingDS[,1]
  a <- computeTraining(trainingDS, class1, class2)
  inv.ctc <- solve(a$ctc)
  tmp1 <- -1/2*(t(a$mt1)%*%inv.ctc%*%a$mt1)
  tmp2 <- -1/2*(t(a$mt2)%*%inv.ctc%*%a$mt2)
  ln.pi1 <- log(a$n1/a$nc)
  ln.pi2 <- log(a$n2/a$nc)
  train1 <- trainingDS[label == class1, -1]
  classifier <- function(data){
    return (c(tmp1)+c(ln.pi1)+as.matrix(data)%*%inv.ctc%*%a$mt1- (c(tmp2)+c(ln.pi2)+as.matrix(data)%*%inv.ctc%*%a$mt2))
  }
  
  
  return( list("classifier"=classifier))
}
#Problem 2 step 2 for QDA
QDA <- function(trainingDS, class1, class2){
  #return classifier that return >0 if data in class1
  label <- trainingDS[,1]
  a <- computeTraining(trainingDS, class1, class2)
  inv.ct1 <- solve(a$ct1)
  inv.ct2 <- solve(a$ct2)
  ln.pi1 <- log(a$n1/a$nc)
  ln.pi2 <- log(a$n2/a$nc)
  s1 <- svd(a$ct1)
  s2 <- svd(a$ct2)
  sum1<-sum(log(s1$d))
  sum2<-sum(log(s2$d))
  linear1 <- -1/2*sum1
  linear2 <- -1/2*sum2
  #print(t(as.matrix(train3[1,]-a$mt1)))
  classifier <- function(data){
    return (c(linear1)+c(ln.pi1)-1/2*(as.matrix(data-a$mt1)%*%inv.ct1%*%t(as.matrix(data-a$mt1)))- (c(linear2)+c(ln.pi2)-1/2*(as.matrix(data-a$mt2)%*%inv.ct2%*%t(as.matrix(data-a$mt2)))))
  }
  return( list("classifier1"=classifier))
}

#Problem 3
#Testing for Training
a <- LDA(zip.train, 3, 5)
cf1 = a$classifier(train3)
cf2 = a$classifier(train5)*-1
trainingResult=c(cf1,cf2)
length(trainingResult)
count=0
for (val in trainingResult){
  if(val>0){
    count = count+1
  }
}
acc=count/length(trainingResult)
print("Accuracy of LDA Training is:")
print(acc)
#LDA Training ACC:0.9934

#QDA
b <- QDA(zip.train, 3, 5)
countQ=0
#Please be patient, takes a long time to run
for (val in c(1:dim(train3)[1])){
  if(b$classifier(train3[val,])[1,1]>0){
    countQ = countQ+1
  }
}
#Please be patient, takes a long time to run
for (val in c(1:dim(train5)[1])){
  if(b$classifier(train5[val,])[1,1]<0){
    countQ = countQ+1
  }
}

lenDS=dim(train3)[1]+dim(train5)[1]
accQ=countQ/lenDS
print("Accuracy of QDA Training is:")
print(accQ)
#QDA Training ACC:1

#Testing for Testing
test3 <- zip.test[test_label == 3 , -1]
test5 <- zip.test[test_label == 5, -1]
testc <- zip.test[test_label == 5|test_label == 3,]
#LDA
a <- LDA(zip.train, 3, 5)
cf1 = a$classifier(test3)
cf2 = a$classifier(test5)*-1
testingResult=c(cf1,cf2)
length(testingResult)
count=0
for (val in testingResult){
  if(val>0){
    count = count+1
  }
}
acc=count/length(testingResult)
print("Accuracy of Testing is:")
print(acc)
#0.9294

#QDA
b <- QDA(zip.train, 3, 5)
countQ=0
#Please be patient, takes a long time to run
for (val in c(1:dim(test3)[1])){
  if(b$classifier(test3[val,])[1,1]>0){
    countQ = countQ+1
  }
}
#Please be patient, takes a long time to run
for (val in c(1:dim(test5)[1])){
  if(b$classifier(test5[val,])[1,1]<0){
    countQ = countQ+1
  }
}
lenDS=dim(test3)[1]+dim(test5)[1]
accQ=countQ/lenDS
print("Accuracy of QDA Training is:")
print(accQ)
#QDA Testing 0.9509
